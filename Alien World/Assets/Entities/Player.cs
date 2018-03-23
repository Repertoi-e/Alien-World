using Alien_World.Physics;

using SharpDX;

static class Player
{
    public static GameEntity CreatePlayer(this GameContext context, float x, float y)
    {
        GameEntity entity = context.CreateEntity();
        entity.AddPosition(x, y);
        entity.AddSprite(SpriteComponent.GetRenderableFromDefinition("Static", "/Assets/Art/Title-Screen/button-start.png", 50, 50));
        entity.AddScript(ScriptComponent.GetLuaScript(entity, "/Assets/Scripts/Player.lua"));
        entity.AddVelocity(Vector2.Zero);
        entity.AddCollision(new Polygon(entity.position, 25, 25), null);
        entity.isStaticBody = false;

        return entity;
    }
}
