using Alien_World.Physics;
using Alien_World.Graphics;

using SharpDX;

static class Player
{
    public static GameEntity CreatePlayer(this GameContext context, float x, float y)
    {
        GameEntity entity = context.CreateEntity();
        entity.AddPosition(x, y);
        entity.AddRenderable(new RenderableInfo
        {
            Reference = PlayerSprites.PlayerRun,
            RenderOffset = new Vector2(-50 / 2, -50 / 2)
        });
        entity.AddScript(ScriptComponent.GetLuaScript(entity, "/Assets/Scripts/Player.lua"));
        entity.AddVelocity(Vector2.Zero);

        /*
           0, -70
          50, -50
          50,  50
           0,  75
         -50,  50
         -50, -50
         */
        entity.AddCollision(new Polygon(entity.position, 50 / 2, 50 / 2), null);
        entity.isStaticBody = false;

        return entity;
    }
}
