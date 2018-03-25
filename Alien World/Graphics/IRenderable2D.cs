using SharpDX;

namespace Alien_World.Graphics
{
    public interface IRenderable2D
    {
        void Render(Vector2 position, IRenderer2D renderer);
    }
}
