using Alien_World.Graphics;

using SharpDX;

public static class PlayerSprites
{
    public static AnimatedSprite PlayerRun = 
        new AnimatedSprite(
            new Vector2(50, 50),
            new TextureSheet("/Assets/Art/Sprites/characters/player/player-run.png", 
                TextureFilter.NEAREST, 1, 8)
            .Regions.ToArray(),
            3).Start();

    public static AnimatedSprite PlayerIdle =
        new AnimatedSprite(
            new Vector2(50, 50),
            new TextureSheet("/Assets/Art/Sprites/characters/player/player-idle.png",
                TextureFilter.NEAREST, 1, 4)
            .Regions.ToArray(),
            13).Start();

    public static AnimatedSprite PlayerJump =
        new AnimatedSprite(
            new Vector2(50, 50),
            new TextureSheet("/Assets/Art/Sprites/characters/player/player-jump.png",
                TextureFilter.NEAREST, 1, 4)
            .Regions.ToArray(),
            1);
}
