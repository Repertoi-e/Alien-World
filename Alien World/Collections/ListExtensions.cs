using System.Linq;
using System.Collections.Generic;

namespace Alien_World.Collections
{
    public static class ListExtensions
    {
        public static void Resize<T>(this List<T> list, int size, T element)
        {
            int current = list.Count;
            if (size < current)
                list.RemoveRange(size, current - size);
            else if (size > current)
            {
                if (size > list.Capacity)
                    list.Capacity = size;
                list.AddRange(Enumerable.Repeat(element, size - current));
            }
        }

        public static void Resize<T>(this List<T> list, int size) where T : new()
        {
            Resize(list, size, new T());
        }
    }
}
