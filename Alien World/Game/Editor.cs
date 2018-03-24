using Alien_World.App;
using Alien_World.Graphics;

namespace Alien_World
{
    public class Editor : Layer
    {
        public Editor() : base(Application.Instance)
        {
            m_EntitySystems
                .Add(new Physics.CollisionSystem(m_EntityContext))
                .Add(new Physics.MovementSystem(m_EntityContext))
                .Add(new Script.ScriptSystem(m_EntityContext))
                .Add(new AnimationSystem(m_EntityContext));

            GameEntity player = m_EntityContext.CreatePlayer(250, 250);
            GameEntity ground = m_EntityContext.CreateTestGround(200, 500);
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
