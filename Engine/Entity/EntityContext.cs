using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Engine.Collections;

namespace Engine.Entity_Component_System
{
    using ComponentMask = BitVector64;

    public class EntityContext : IDisposable
    {
        List<BaseComponentPool> m_ComponentPools = new List<BaseComponentPool>(64);
        List<ComponentMask> m_EntityComponentMasks = new List<ComponentMask>();
        List<BaseComponentHelper> m_ComponentHelpers = new List<BaseComponentHelper>();
        List<uint> m_EntityVersions = new List<uint>();
        List<uint> m_FreeList = new List<uint>();
        Dictionary<ComponentMask, EntityGroup> m_Groups = new Dictionary<ComponentMask, EntityGroup>();
        uint m_IndexCounter = 0;
        bool m_Disposed = false;

        int Capacity => m_EntityComponentMasks.Count;
        int ManagedEntities => Capacity - m_FreeList.Count;

        public EntityContext()
        {
            if (!SystemIndexer.Indexed)
                SystemIndexer.Index();
            if (!ComponentIndexer.Indexed)
                ComponentIndexer.Index();
        }

        public void Reset()
        {
            m_ComponentPools.Clear();
            m_EntityComponentMasks.Clear();
            m_ComponentHelpers.Clear();
            m_EntityVersions.Clear();
            m_FreeList.Clear();
            m_IndexCounter = 0;
        }

        public void Dispose()
        {
            if (!m_Disposed)
            {
                Reset();
                m_Disposed = true;
            }
        }

        public Entity Create()
        {
            uint index, version;
            if (!m_FreeList.Any())
            {
                index = m_IndexCounter++;
                AccommodateEntity(index);
                version = m_EntityVersions[(int)index] = 1;
            }
            else
            {
                index = m_FreeList.Last();
                m_FreeList.RemoveAt(m_FreeList.Count - 1);
                version = m_EntityVersions[(int)index];
            }
            return new Entity(this, new Entity.Id(index, version));
        }

        public Entity CreateFromCopy(Entity original)
        {
            Debug.Assert((bool)original);
            Entity clone = Create();
            ComponentMask mask = original.ComponentMask();
            for (int i = 0; i < m_ComponentHelpers.Count; i++)
            {
                BaseComponentHelper helper = m_ComponentHelpers[i];
                if (helper != null && mask[i])
                    helper.CopyComponentTo(original, clone);
            }
            return clone;
        }

        public void Destroy(Entity.Id entity)
        {
            AssertValidID(entity);
            int index = (int)entity.Index;
            ComponentMask mask = m_EntityComponentMasks[index];
            for (int i = 0; i < m_ComponentHelpers.Count; i++)
            {
                BaseComponentHelper helper = m_ComponentHelpers[i];
                if (helper != null && mask[i])
                    helper.RemoveComponent(new Entity(this, entity));
            }
            m_EntityComponentMasks[index].Reset();
            m_EntityVersions[index]++;
            m_FreeList.Add((uint)index);
        }

        public EntityGroup GetGroup(ComponentMask mask)
        {
            if (!m_Groups.TryGetValue(mask, out EntityGroup result))
            {
                result = new EntityGroup(mask);
                for (uint i = 0; i < m_EntityComponentMasks.Count; i++)
                    result.HandleEntity(Get(CreateID(i)));

                m_Groups.Add(mask, result);
            }
            return result;
        }

        Entity.Id CreateID(uint index)
        {
            return new Entity.Id(index, m_EntityVersions[(int)index]);
        }

        internal ComponentMask ComponentMask(Entity.Id id)
        {
            AssertValidID(id);
            return m_EntityComponentMasks.ElementAt((int)id.Index);
        }

        internal bool Matches(Entity.Id m_ID, ComponentMask mask)
        {
            return (m_EntityComponentMasks[(int)m_ID.Index] & mask) == mask;
        }

        internal void Remove<C>(Entity.Id id) where C : class, IComponent, new()
        {
            int entityId = (int)id.Index;
            int family = ComponentIndexer.GetFamily<C>();

            ComponentMask mask = m_EntityComponentMasks[entityId];
            mask[1 << family] = false;
            m_EntityComponentMasks[entityId] = mask;

            ComponentPool<C> pool = (ComponentPool<C>)m_ComponentPools[family];
            pool.Destroy(entityId);
        }

        internal bool Valid(Entity.Id id)
        {
            return (int)id.Index < m_EntityVersions.Count && m_EntityVersions[(int)id.Index] == id.Version;
        }

        internal bool HasComponent<C>(Entity.Id id)
        {
            AssertValidID(id);
            int family = ComponentIndexer.GetFamily<C>();
            // We don't bother checking the component mask, as we return a nullptr anyway.
            if (family >= m_ComponentPools.Count)
                return false;
            BaseComponentPool pool = m_ComponentPools[family];
            if (pool == null || !m_EntityComponentMasks[(int)id.Index][family])
                return false;
            return true;
        }

        internal C Component<C>(Entity.Id id) where C : class, IComponent, new()
        {
            AssertValidID(id);
            int family = ComponentIndexer.GetFamily<C>();
            if (family >= m_ComponentPools.Count)
                return default(C);
            BaseComponentPool pool = m_ComponentPools[family];
            if (pool == null || !m_EntityComponentMasks[(int)id.Index][1 << family])
                return default(C);
            return ((ComponentPool<C>)pool).Get((int)id.Index);
        }

        internal C Assign<C>(Entity.Id id, C c) where C : class, IComponent, new()
        {
            AssertValidID(id);
            int family = ComponentIndexer.GetFamily<C>();
            int entityId = (int)id.Index;
            ComponentMask mask = m_EntityComponentMasks[entityId];
            Debug.Assert(!mask[1 << family]);

            ComponentPool<C> pool = AccommodateComponent<C>();
            pool.Put(c, entityId);

            mask[1 << family] = true;
            m_EntityComponentMasks[entityId] = mask;

            return c;
        }

        private Entity Get(Entity.Id id)
        {
            AssertValidID(id);
            return new Entity(this, id);
        }

        private ComponentPool<C> AccommodateComponent<C>() where C : class, IComponent, new()
        {
            int family = ComponentIndexer.GetFamily<C>();
            if (m_ComponentPools.Count <= family)
                m_ComponentPools.Resize(family + 1, null);
            if (m_ComponentPools[family] == null)
            {
                ComponentPool<C> pool = new ComponentPool<C>();
                pool.Expand((int)m_IndexCounter);
                m_ComponentPools[family] = pool;
            }
            if (m_ComponentHelpers.Count <= family)
                m_ComponentHelpers.Resize(family + 1, null);
            if (m_ComponentHelpers[family] == null)
            {
                ComponentHelper<C> helper = new ComponentHelper<C>();
                m_ComponentHelpers[family] = helper;
            }
            return (ComponentPool<C>)m_ComponentPools[family];
        }

        private void AccommodateEntity(uint index)
        {
            if (m_EntityComponentMasks.Count <= index)
            {
                m_EntityComponentMasks.Resize((int)index + 1);
                m_EntityVersions.Resize((int)index + 1);

                foreach (BaseComponentPool pool in m_ComponentPools)
                    if (pool != null)
                        pool.Expand((int)index + 1);
            }
        }

        private void AssertValidID(Entity.Id id)
        {
            int index = (int)id.Index;
            Debug.Assert(index < m_EntityComponentMasks.Count, "entity ID outside vector range");
            Debug.Assert(m_EntityVersions[index] == id.Version, "attempt to access Entity via a stale entity id");
        }
    }
}
