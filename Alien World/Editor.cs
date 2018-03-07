using System;

using Engine.Entity_Component_System;
using Engine.Entity_Component_System.Components;

using Engine.App;
using Engine.Graphics;
using Engine.Resource_Manager;
using Engine.Input_Events;

namespace Alien_World
{
    public class Editor : Layer
    {
        public Editor() : base(Application.Instance)
        {
            Entity player = EntityFactory.GetFromJson(m_EntityManager, ResourceLoader.LoadTextFile("/Resources/Player.json"));
            Entity player2 = EntityFactory.GetFromJson(m_EntityManager, ResourceLoader.LoadTextFile("/Resources/Player2.json"));
            for (int i = 0; i < 0; i++)
                player2 = m_EntityManager.CreateFromCopy(player2);

            PositionComponent comp = player.PositionComponent();
            comp.Value.X += 450;

            Console.WriteLine(player.Component<PositionComponent>().Value.ToString());
        }

        protected override void Dispose(bool managed)
        {
            base.Dispose(managed);
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
        }

        public override void OnRender(Renderer2D renderer2D)
        {
            base.OnRender(renderer2D);
        }
    }
}
