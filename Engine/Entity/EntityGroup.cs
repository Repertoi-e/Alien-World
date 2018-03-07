using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Entity_Component_System
{
    using ComponentMask = BitVector64;

    public class EntityGroup : IEnumerable<Entity>
    {
        HashSet<Entity> m_Entities = new HashSet<Entity>();
        Entity[] m_EntitiesCache;

        public ComponentMask Matcher { get; }

        public EntityGroup(ComponentMask matcher)
        {
            Matcher = matcher;
        }

        public void HandleEntity(Entity entity)
        {
            if (entity.Matches(Matcher))
            {
                if ((bool)entity)
                {
                    bool added = m_Entities.Add(entity);
                    if (added)
                        m_EntitiesCache = null;
                }
            }
            else
            {
                bool removed = m_Entities.Remove(entity);
                if (removed)
                    m_EntitiesCache = null;
            }
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
