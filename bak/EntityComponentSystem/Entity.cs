﻿
namespace Alien_World.Entity_Component_System
{
    using Alien_World.Entity_Component_System.Components;

    using ComponentMask = BitVector64;

	public delegate void EntityComponentChanged(Entity entity, int index, IComponent component);

    public class Entity
    {
        public class Id
        {
            ulong m_ID;

            public Id(ulong id) => m_ID = id;
            public Id(uint index, uint version) => m_ID = ((ulong)version << 32 | index);

            public ulong ID => m_ID;

            public ulong Index => m_ID & 0xffffffffUL;
            public ulong Version => m_ID >> 32;

            public static bool operator ==(Id one, Id other) { return one.m_ID == other.m_ID; }
            public static bool operator !=(Id one, Id other) { return one.m_ID != other.m_ID; }

            public static bool operator <(Id one, Id other) { return one.m_ID < other.m_ID; }
            public static bool operator >(Id one, Id other) { return one.m_ID > other.m_ID; }

            public override bool Equals(object obj)
            {
                Id id = obj as Id;
                return id != null && m_ID == id.m_ID;
            }

            public override string ToString()
            {
                return m_ID.ToString();
            }

            public override int GetHashCode()
            {
                return (int)(Index ^ Version);
            }
        }

		public event EntityComponentChanged OnComponentAdded;
        public event EntityComponentChanged OnComponentRemoved;

        public static readonly Id Invalid = new Id(0);

        Id m_ID = Invalid;
        EntityContext m_EntityManager;

        public Id ID => m_ID;

        public Entity(EntityContext entityManager, Id id)
        {
            m_EntityManager = entityManager;
            m_ID = id;
        }

        public bool Valid()
        {
            return m_EntityManager != null && m_EntityManager.Valid(m_ID);
        }

        public void Invalidate()
        {
            m_ID = Invalid;
            m_EntityManager = null;
        }

        public void Assign<C>(C c) where C : class, IComponent, new()
        {
            OnComponentAdded?.Invoke(this, m_EntityManager.Assign(m_ID, c), c);
        }

        public void Remove<C>() where C : class, IComponent, new()
        {
            OnComponentRemoved?.Invoke(this, m_EntityManager.Remove<C>(m_ID), null);
        }

        public C Component<C>() where C : class, IComponent, new()
        {
            return m_EntityManager.Component<C>(m_ID);
        }

        public bool HasComponent<C>() where C : class, IComponent
        {
            return m_EntityManager.HasComponent<C>(m_ID);
        }

        internal ComponentMask ComponentMask()
        {
            return m_EntityManager.ComponentMask(m_ID);
        }

        internal bool Matches(ComponentMask mask)
        {
            return m_EntityManager.Matches(m_ID, mask);
        }

        public override bool Equals(object obj)
        {
            var entity = obj as Entity;
            return entity != null && m_ID == entity.m_ID;
        }

        public override int GetHashCode()
        {
            return m_ID.GetHashCode();
        }

        public static bool operator ==(Entity one, Entity other) => one.m_EntityManager == other.m_EntityManager && one.m_ID == other.m_ID;
        public static bool operator !=(Entity one, Entity other) => !(one == other);
        public static bool operator <(Entity one, Entity other) => one.m_ID < other.m_ID;
        public static bool operator >(Entity one, Entity other) => one.m_ID > other.m_ID;
        public static explicit operator bool(Entity entity) { return entity.Valid(); }
    }
}

