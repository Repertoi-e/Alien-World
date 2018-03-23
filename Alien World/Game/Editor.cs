using Alien_World.App;
using Alien_World.Graphics;

namespace Alien_World
{
    public class Editor : Layer
    {
        public Editor() : base(Application.Instance)
        {
            m_EntitySystemManager
                .Add(new Physics.CollisionSystem(m_EntityManager))
                .Add(new Physics.MovementSystem(m_EntityManager))
                .Add(new Script.ScriptSystem(m_EntityManager))
                .Add(new AnimationSystem(m_EntityManager));

            GameEntity player = m_EntityManager.CreatePlayer(250, 250);
            GameEntity ground = m_EntityManager.CreateTestGround(200, 500);
        }

        protected override void Dispose(bool managed)
        {
            base.Dispose(managed);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnRender(Renderer2D renderer2D)
        {

        }
    }
}
