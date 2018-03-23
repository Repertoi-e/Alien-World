using System;
using System.Collections;
using System.Collections.Generic;

namespace Alien_World.Entity_Component_System.Group
{
    using ComponentMask = BitVector64;

    public delegate void GroupChanged(EntityGroup group, Entity entity, int index, 
        IComponent component);

    public delegate void GroupUpdated(EntityGroup group, Entity entity, int index,
        IComponent previousComponent, IComponent newComponent);

    public class EntityGroup : IEnumerable<Entity>
    {
        event GroupChanged OnEntityAdded;
        event GroupChanged OnEntityRemoved;
        event GroupUpdated OnEntityUpdated;

        HashSet<Entity> m_Entities = new HashSet<Entity>();
        Entity[] m_EntitiesCache;

        public ComponentMask Matcher { get; }

        public EntityGroup(ComponentMask matcher)
        {
            Matcher = matcher;
        }

        public void HandleEntitySilently(Entity entity)
        {
            if (entity.Matches(Matcher))
                AddEntitySilently(entity);
            else
                RemoveEntitySilently(entity);
        }

        void AddEntity(Entity entity, int index, IComponent component)
        {
            if (AddEntitySilently(entity) && OnEntityAdded != null)
                OnEntityAdded(this, entity, index, component);
        }

        bool AddEntitySilently(Entity entity)
        {
            if ((bool)entity)
            {
                var added = m_Entities.Add(entity);
                if (added)
                    m_EntitiesCache = null;

                return added;
            }

            return false;
        }

        void RemoveEntity(Entity entity, int index, IComponent component)
        {
            var removed = m_Entities.Remove(entity);
            if (removed)
            {
                m_EntitiesCache = null;
                if (OnEntityRemoved != null)
                    OnEntityRemoved(this, entity, index, component);
            }
        }

        bool RemoveEntitySilently(Entity entity)
        {
            var removed = m_Entities.Remove(entity);
            if (removed)
                m_EntitiesCache = null;

            return removed;
        }

        public GroupChanged HandleEntity(Entity entity)
        {
            return entity.Matches(Matcher)
                ? (AddEntitySilently(entity) ? OnEntityAdded : null)
                : (RemoveEntitySilently(entity) ? OnEntityRemoved : null);
        }

        public Entity[] GetEntities()
        {
            if (m_EntitiesCache == null)
            {
                m_EntitiesCache = new Entity[m_Entities.Count];
                m_Entities.CopyTo(m_EntitiesCache);
            }

            return m_EntitiesCache;
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return m_Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
