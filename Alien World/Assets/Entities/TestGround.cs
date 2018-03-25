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
        Text text = new Text("Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
            "Maecenas ac enim ac ipsum tincidunt ultricies. Ut sed tortor ac felis consequat " +
            "tincidunt accumsan in sem. Vestibulum laoreet ante diam, id malesuada ipsum " +
            "pharetra nec. Praesent quis tortor vitae felis posuere hendrerit. Maecenas " +
            "scelerisque auctor augue vel interdum. Etiam feugiat sodales tellus, sit amet " +
            "volutpat ante. Curabitur gravida, lectus et mattis lobortis, dolor odio dictum " +
            "ex, ac ultrices tortor elit accumsan libero.", "Segoe UI", 13, 300, 300);
        text.TextFormat.TextAlignment = TextAlignment.Justified;
        text.UpdateTextLayout();
        entity.AddRenderable(new RenderableInfo { Reference = text });
        entity.OnComponentRemoved += (IEntity e, int index, IComponent component) =>
        {
            if (component is RenderableComponent comp)
                ((Text)comp.Info.Reference).Dispose();
        };

        entity.AddVelocity(Vector2.Zero);

        entity.AddCollision(new Polygon(entity.position, 450, 10), null);
        entity.isStaticBody = true;
        
        return entity;
    }
}
