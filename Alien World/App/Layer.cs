using System;

using Entitas;

using Alien_World.Graphics;

namespace Alien_World.App
{
    public abstract class Layer : IDisposable
    {
        protected Application m_App;
        protected IRenderer2D m_Renderer2D;
        protected GameContext m_EntityContext = new GameContext();
        protected Systems m_EntitySystems = new Systems();
        protected IGroup<GameEntity> m_RenderableEntities;
        protected bool m_Disposed = false;

        protected virtual void Dispose(bool managed)
        {
            if (!m_Disposed)
            {
                if (managed)
                {
                    m_Renderer2D.Dispose();
                    m_EntityContext.DestroyAllEntities();
                }
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
            m_Renderer2D = new BatchRenderer2D();
            m_RenderableEntities = m_EntityContext.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Renderable));
        }

        public void _Render()
        {
            OnPreRender();

            m_Renderer2D.Begin();

            OnEarlyRender(m_Renderer2D);
            foreach (GameEntity entity in m_RenderableEntities.GetEntities())
                entity.renderable.Render(entity.position, m_Renderer2D);

            OnLateRender(m_Renderer2D);
            
            m_Renderer2D.Present();

            OnPostRender();
        }

        public void _Update()
        {
            OnUpdate();
            m_EntitySystems.Execute();
            m_EntitySystems.Cleanup();
            OnLateUpdate();
        }

        public virtual void OnUpdate() { }
        public virtual void OnLateUpdate() { }
        public virtual void OnPreRender() { }
        public virtual void OnEarlyRender(IRenderer2D renderer2D) { }
        public virtual void OnLateRender(IRenderer2D renderer2D) { }
        public virtual void OnPostRender() { }
    }
}
