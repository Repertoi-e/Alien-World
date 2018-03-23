using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Alien_World.Collections
{
    public class EnumerableCollection<T> : ICollection<T>
    {
        private readonly IEnumerable<T> m_Enumerable;

        public int Count { get; private set; }

        public bool IsReadOnly => true;

        public EnumerableCollection(IEnumerable<T> enumerable, int count)
        {
            m_Enumerable = enumerable;
            Count = count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_Enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return this.Any(v => item.Equals(v));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length < Count + arrayIndex)
                throw new ArgumentOutOfRangeException(nameof(array), $"the supplied array (of size {array.Length}) cannot contain {Count} items on index {arrayIndex}");
            foreach (var item in m_Enumerable)
                array[arrayIndex++] = item;
        }

        public void Add(T item) { throw new NotSupportedException(); }
        public void Clear() { throw new NotSupportedException(); }
        public bool Remove(T item) { throw new NotSupportedException(); }
    }
}
