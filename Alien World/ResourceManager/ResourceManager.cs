using System;
using System.Collections.Generic;

namespace Alien_World.Resource_Manager
{
    public static class ResourceManager<T> where T : Resource
    {
        static Dictionary<string, T> m_Resources = new Dictionary<string, T>();

        public static T Get(string name)
        {
            if (!m_Resources.ContainsKey(name))
            {
                Console.WriteLine($"Resource not found: \"{name}\".");
                return default(T);
            }

            return m_Resources[name];
        }

        public static void Add(T resource)
        {
            if (m_Resources.ContainsKey(resource.Name))
            {
                Console.WriteLine($"Resource \"{resource.Name}\" already exists.");
                return;
            }
            Console.WriteLine($"Adding resource \"{resource.Name}\"...");
            m_Resources[resource.Name] = resource;
        }

        public static void Clear()
        {
            foreach (KeyValuePair<string, T> res in m_Resources)
                res.Value.Dispose();
            m_Resources.Clear();
        }
    }
}
