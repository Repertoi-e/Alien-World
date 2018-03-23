using Alien_World.Graphics;
using Alien_World.Entity_Component_System.Components;

namespace Alien_World.Entity_Component_System
{
    public class Render2DSystem : System
    {
        Renderer2D m_Renderer;
    
        public Render2DSystem(Renderer2D renderer)
        {
            m_Renderer = renderer;
        }
        
        public override void Configure() { }
        
        public override void Update(EntityContext entities, float dt = 0.0f)
        {
            foreach (Entity entity in entities.GetGroup(ComponentMatcher.Get<PositionComponent, SpriteComponent>()))
            {
                PositionComponent pos = entity.Component<PositionComponent>();
                SpriteComponent renderable = entity.Component<SpriteComponent>();
                m_Renderer.Submit(pos.Value, renderable.Value);
            }
        }
    }
}
