using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Alien_World.Entity_Component_System
{
    public abstract class System
    {
        public abstract void Configure();

        public abstract void Update(EntityContext entities, float dt);
    }

    public class EntitySystemManager
    {
        EntityContext m_EntityManager = new EntityContext();
        Dictionary<int, System> m_Systems = new Dictionary<int, System>();
        bool m_Initialized = false;

        public EntitySystemManager(EntityContext entityManager) => m_EntityManager = entityManager;

        public void AddSystem<S>() where S : System, new()
        {
            m_Systems[SystemIndexer.GetFamily<S>()] = new S();
        }

        public void AddSystem<S>(params object[] parameters) where S : System, new()
        {
            m_Systems[SystemIndexer.GetFamily<S>()] = (S)Activator.CreateInstance(typeof(S), parameters);
        }

        public void Configure()
        {
            foreach (KeyValuePair<int, System> system in m_Systems)
                system.Value.Configure();
            m_Initialized = true;
        }

        public void Update(float dt)
        {
            Debug.Assert(m_Initialized, "System manager not configured.");
            foreach (KeyValuePair<int, System> system in m_Systems)
                system.Value.Update(m_EntityManager, dt);
        }
    }

    public static class SystemIndexer
    {
        static Dictionary<Type, int> m_Systems = new Dictionary<Type, int>();
        static bool m_Indexed = false;

        public static bool Indexed => m_Indexed;

        public static void Index()
        {
            int identifier = 0;
            foreach (Type type in Assembly.GetAssembly(typeof(System)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(System))))
                m_Systems[type] = identifier++;
            m_Indexed = true;
        }

        public static int GetFamily<C>()
        {
            return m_Systems[typeof(C)];
        }
    }
}
