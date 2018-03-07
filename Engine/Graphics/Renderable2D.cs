using SharpDX;

namespace Engine.Graphics
{
    public class Renderable2D
    {
        Vector2 m_Size;
        uint m_Color = 0xff_ffffff;
        Texture m_Texture = null;
        Vector2[] m_UVs = s_DefaultUVs;

        static readonly Vector2[] s_DefaultUVs = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

        public Renderable2D(Vector2 size, Texture texture)
        {
            m_Size = size;
            m_Texture = texture;
        }

        public Renderable2D(Vector2 size, uint color)
        {
            m_Size = size;
            m_Color = color;
        }

        public Vector2 Size { get { return m_Size; } set { m_Size = value; } }
        public uint Color { get { return m_Color; } set { m_Color = value; } }
        public Texture Texture { get { return m_Texture; } set { m_Texture = value; } }
        public Vector2[] UVs { get { return m_UVs; } set { m_UVs = value; } }

        public static Vector2[] DefaultUVs { get { return s_DefaultUVs; } }
    }
}
