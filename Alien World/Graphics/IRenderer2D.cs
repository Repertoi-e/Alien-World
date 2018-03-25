using System;
using System.Collections.Generic;

using SharpDX;

namespace Alien_World.Graphics
{
    public interface IRenderer2D : IDisposable
    {
        void Begin();
        void Submit(Vector2 position, Sprite sprite);
        void DrawLine(float x0, float y0, float x1, float y1, uint color, float thickness);
        void DrawPolygon(List<Vector2> vertices, uint color, float thickness = 0.5f);
        void FillPolygon(List<Vector2> vertices, uint color);
        void FillRect(float x, float y, float width, float height, uint color);
        void Present();
    }
}
