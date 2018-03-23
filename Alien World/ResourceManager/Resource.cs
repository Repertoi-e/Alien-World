using System;

namespace Alien_World.Resource_Manager
{
    public class Resource : IDisposable
    {
        public string Name { get; protected set; }
        public string FilePath { get; protected set; }

        protected bool m_Disposed = false;

        ~Resource()
        {
            DisposeUnmanaged();
        }

        protected virtual void DisposeManaged()
        {
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        public void Dispose()
        {
            DisposeManaged();
            GC.SuppressFinalize(this);
        }
    }
}
