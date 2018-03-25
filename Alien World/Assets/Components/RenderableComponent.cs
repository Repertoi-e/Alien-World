using Alien_World.Graphics;

using Entitas;

using SharpDX;

public struct RenderableComponent : IComponent
{
    public RenderableInfo Info;

    public void Render(Vector2 position, IRenderer2D renderer)
    {
        Info.Reference.Render(position + Info.RenderOffset, renderer);
    }
}