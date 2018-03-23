using Alien_World.Physics;

using SharpDX;

static class TestGround
{
    public static GameEntity CreateTestGround(this GameContext context, float x, float y)
    {
        GameEntity entity = context.CreateEntity();
        entity.AddPosition(x, y);
        entity.AddSprite(SpriteComponent.GetRenderableFromDefinition("Static", "/Assets/Art/Title-Screen/button-start.png", 900, 20));
        entity.AddVelocity(Vector2.Zero);
        entity.AddCollision(new Polygon(entity.position, 450, 10), null);
        entity.isStaticBody = true;

        return entity;
    }
}
