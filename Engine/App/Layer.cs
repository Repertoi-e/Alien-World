using Engine.Entity_Component_System;
using Engine.Graphics;
using System;

namespace Engine.App
{
    public abstract class Layer : IDisposable
    {
        protected Application m_App;
        protected Renderer2D m_Renderer2D;
        protected EntityContext m_EntityManager;
        protected EntitySystemManager m_EntitySystemManager;
        Render2DSystem m_RenderSystem;
        bool m_Disposed = false;

        protected virtual void Dispose(bool managed)
        {
            if (!m_Disposed)
            {
                if (managed)
                    m_Renderer2D.Dispose();
                m_Disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public Layer(Application program)
        {
            m_App = program;
            m_Renderer2D = new Renderer2D();
            m_EntityManager = new EntityContext();
            m_EntitySystemManager = new EntitySystemManager(m_EntityManager);
            m_RenderSystem = new Render2DSystem(m_Renderer2D);
        }

        public void OnRender()
        {
            m_Renderer2D.Begin();
            
            m_RenderSystem.Update(m_EntityManager);
            OnRender(m_Renderer2D);
            
            m_Renderer2D.End();
            m_Renderer2D.Present();
        }

        public virtual void OnUpdate(float dt) { }
        public virtual void OnRender(Renderer2D renderer2D) { }
    }
}
