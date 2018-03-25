using Alien_World.App;
using Alien_World.Graphics;

namespace Alien_World
{
    public class Editor : Layer
    {
        protected IRenderer2D m_CollisionBoxRenderer = new BatchRenderer2D();

        public Editor() : base(Application.Instance)
        {
            m_EntitySystems
                .Add(new Physics.CollisionSystem(m_EntityContext))
                .Add(new Physics.MovementSystem(m_EntityContext))
                .Add(new Script.ScriptSystem(m_EntityContext))
                .Add(new AnimationSystem(m_EntityContext));

            m_EntityContext.CreatePlayer(250, 250);
            m_EntityContext.CreateTestGround(200, 500);
        }

        protected override void Dispose(bool managed)
        {
            if (!m_Disposed)
            {
                if (managed)
                    m_CollisionBoxRenderer.Dispose();
            }
            base.Dispose(managed);
        }

        public override void OnPostRender()
        {
            Context.Instance.SetWireframe(true);
            m_CollisionBoxRenderer.Begin();
            for (int i = 0; i < m_RenderableEntities.GetEntities().Length; i++)
            {
                GameEntity entity = m_RenderableEntities.GetEntities()[i];
                if (entity.hasCollision)
                    m_CollisionBoxRenderer.FillPolygon(entity.collision.CollisionBounds.Vertices, 0xff00ffff);
            }
            m_CollisionBoxRenderer.Present();
            Context.Instance.SetWireframe(false);
        }
    }
}
