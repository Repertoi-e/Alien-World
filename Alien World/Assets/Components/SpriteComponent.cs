using System;

using Entitas;

using Alien_World.Graphics;
using Alien_World.Resource_Manager;

public struct SpriteComponent : IComponent
{
    public IRenderable2D Renderable;
    public bool Flipped;

    public bool IsAnimated { get
        {
            return Renderable is AnimatedSprite;
        } }

    public AnimatedSprite GetAsAnimated { get
        {
            if (Renderable is AnimatedSprite sprite)
                return sprite;
            return null;
        } }

    /// <param name="spriteType">either static/...(TODO)</param>
    /// <param name="resource">resource path</param>
    public static Sprite GetRenderableFromDefinition(string spriteType, string resource, float width, float height)
    {
        if (spriteType == null)
             throw new NullReferenceException("spriteType");

        if (spriteType.ToLower().Equals("static"))
        {
            if (resource == null)
                throw new NullReferenceException("resource");
            if (ResourceManager<Texture>.Get(resource) is null)
                ResourceManager<Texture>.Add(ResourceLoader.LoadTexture(resource, TextureFilter.NEAREST));
            return new Sprite(new SharpDX.Vector2(width, height), ResourceManager<Texture>.Get(resource));
        }
        return new Sprite(new SharpDX.Vector2(width, height), 0xffff00ff);
    }
}