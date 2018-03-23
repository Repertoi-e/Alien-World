using System;

using Alien_World.App;
using Alien_World.Graphics;
using Alien_World.Resource_Manager;
using Alien_World.Input_Events;

using Entitas;

namespace Alien_World
{
    public class Editor : Layer
    {
        public Editor() : base(Application.Instance)
        {
            m_EntitySystemManager
                .Add(new Physics.CollisionSystem(m_EntityManager))
                .Add(new Physics.MovementSystem(m_EntityManager))
                .Add(new Script.ScriptSystem(m_EntityManager));

            GameEntity player = m_EntityManager.CreatePlayer(250, 250);
            GameEntity ground = m_EntityManager.CreateTestGround(200, 500);
            //for (int i = 0; i < 0; i++)
            //    player2 = m_EntityManager.CreateFromCopy(player2);
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
