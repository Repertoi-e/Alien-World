using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Alien_World.Collections
{
    public class ObjectPool<T>
    {
        private ConcurrentBag<T> m_Objects;
        private Func<T> m_ObjectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null)
                throw new ArgumentNullException("objectGenerator");
            m_Objects = new ConcurrentBag<T>();
            m_ObjectGenerator = objectGenerator;
        }

        public T Get()
        {
            if (m_Objects.TryTake(out T item))
                return item;
            return m_ObjectGenerator();
        }

        public void Push(T item)
        {
            m_Objects.Add(item);
        }
    }
}
