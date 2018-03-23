using System;

using Entitas;

using Alien_World.Graphics;

namespace Alien_World.App
{
    public abstract class Layer : IDisposable
    {
        protected Application m_App;
        protected Renderer2D m_Renderer2D;
        protected GameContext m_EntityManager = new GameContext();
        protected Systems m_EntitySystemManager = new Systems();
        IGroup<GameEntity> m_RenderableEntities;
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
            m_RenderableEntities = m_EntityManager.GetGroup(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Sprite));
        }

        public void _Render()
        {
            m_Renderer2D.Begin();

            OnPreRender(m_Renderer2D);
            foreach (GameEntity entity in m_RenderableEntities.GetEntities())
            {
                m_Renderer2D.Submit(entity.position - entity.sprite.Renderable.Size / 2, entity.sprite.Renderable);
                if (entity.hasCollision)
                    m_Renderer2D.DrawPolygon(entity.collision.CollisionBounds.Vertices, 0xff00ffff);
            }
            OnRender(m_Renderer2D);
            
            m_Renderer2D.End();
            m_Renderer2D.Present();
        }

        public void _Update()
        {
            m_EntitySystemManager.Execute();
            OnUpdate();
        }

        public virtual void OnUpdate() { }
        public virtual void OnPreRender(Renderer2D renderer2D) { }
        public virtual void OnRender(Renderer2D renderer2D) { }
    }
}
