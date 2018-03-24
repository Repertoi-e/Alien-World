using Alien_World.Physics;
using Alien_World.Graphics;

using Entitas;

using SharpDX;
using SharpDX.DirectWrite;

static class TestGround
{
    public static GameEntity CreateTestGround(this GameContext context, float x, float y)
    {
        GameEntity entity = context.CreateEntity();
        entity.AddPosition(x, y);
        //entity.AddSprite(SpriteComponent.GetRenderableFromDefinition("Static", "/Assets/Art/Title-Screen/button-start.png", 900, 20), false);
        entity.AddSprite(new Text(@"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas ac enim ac ipsum tincidunt ultricies. Ut sed tortor ac felis consequat tincidunt accumsan in sem. Vestibulum laoreet ante diam, id malesuada ipsum pharetra nec. Praesent quis tortor vitae felis posuere hendrerit. Maecenas scelerisque auctor augue vel interdum. Etiam feugiat sodales tellus, sit amet volutpat ante. Curabitur gravida, lectus et mattis lobortis, dolor odio dictum ex, ac ultrices tortor elit accumsan libero.", new TextFormat(Context.Instance.DWriteFactory, "Segoe UI", 13)
        {
            TextAlignment = TextAlignment.Trailing
        }, 300, 300), false);
        entity.AddVelocity(Vector2.Zero);
        entity.AddCollision(new Polygon(entity.position, 450, 10), null);
        entity.isStaticBody = true;

        entity.OnComponentRemoved += (IEntity e, int index, IComponent component) =>
        {
            if (component is SpriteComponent comp)
                ((Text)comp.Renderable).Dispose();
        };

        return entity;
    }
}
