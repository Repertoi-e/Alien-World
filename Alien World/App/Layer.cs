using System;

using Entitas;

using Alien_World.Graphics;

namespace Alien_World.App
{
    public abstract class Layer : IDisposable
    {
        protected Application m_App;
        protected Renderer2D m_Renderer2D, m_CollisionBoxRenderer;
        protected GameContext m_EntityContext = new GameContext();
        protected Systems m_EntitySystems = new Systems();
        IGroup<GameEntity> m_RenderableEntities;
        bool m_Disposed = false;

        protected virtual void Dispose(bool managed)
        {
            if (!m_Disposed)
            {
                if (managed)
                {
                    m_Renderer2D.Dispose();
                    m_CollisionBoxRenderer.Dispose();
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
            m_Renderer2D = new Renderer2D();
            m_CollisionBoxRenderer = new Renderer2D();
            m_RenderableEntities = m_EntityContext.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Sprite));
        }

        public void _Render()
        {
            m_Renderer2D.Begin();

            OnPreRender(m_Renderer2D);
            foreach (GameEntity entity in m_RenderableEntities.GetEntities())
                entity.sprite.Renderable.Render(entity.position, m_Renderer2D);

            OnRender(m_Renderer2D);
            
            m_Renderer2D.Present();

            Renderer.Instance.SetWireframe(true);
            m_CollisionBoxRenderer.Begin();
            for (int i = 0; i < m_RenderableEntities.GetEntities().Length; i++)
            {
                GameEntity entity = m_RenderableEntities.GetEntities()[i];
                if (entity.hasCollision)
                {
                    m_CollisionBoxRenderer.FillPolygon(entity.collision.CollisionBounds.Vertices, 0xff00ffff);
                }
            }

            m_CollisionBoxRenderer.Present();
            Renderer.Instance.SetWireframe(false);
        }

        public void _Update()
        {
            m_EntitySystems.Execute();
            OnUpdate();
        }

        public virtual void OnUpdate() { }
        public virtual void OnPreRender(Renderer2D renderer2D) { }
        public virtual void OnRender(Renderer2D renderer2D) { }
    }
}
