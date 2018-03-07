using System.Collections.Generic;
using System.Diagnostics;

using Engine.Collections;

namespace Engine.Entity_Component_System
{
    public abstract class BaseComponentPool
    {
        public abstract void Expand(int toSize);
    }

    public class ComponentPool<C> : BaseComponentPool where C : class, IComponent, new()
    {
        List<C> m_Objects = new List<C>();

        /// Ensure at least n elements will fit in the pool.
        public override void Expand(int toSize)
        {
            if (toSize > m_Objects.Count)
                m_Objects.Resize(toSize);
        }

        public unsafe void Put(C c, int index)
        {
            Debug.Assert(index < m_Objects.Count);
            m_Objects[index] = (C)c.Clone();
        }

        public C Get(int index)
        {
            Debug.Assert(index < m_Objects.Count);
            return m_Objects[index];
        }

        public void Destroy(int index)
        {
            Debug.Assert(index < m_Objects.Count);
            m_Objects[index].Dispose();
        }
    }
}
