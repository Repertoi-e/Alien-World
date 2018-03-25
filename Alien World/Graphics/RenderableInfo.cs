using SharpDX;

namespace Alien_World.Graphics
{
    public enum RenderableType
    {
        Unknown, Sprite, AnimatedSprite, Text
    }

    public class RenderableInfo
    {
        public bool Flipped = false;
        public Vector2 RenderOffset = Vector2.Zero;

        RenderableType m_CachedType = RenderableType.Unknown;
        public RenderableType Type
        {
            get
            {
                if (m_CachedType != RenderableType.Unknown)
                    return m_CachedType;
                if (Reference is Sprite)
                    m_CachedType = RenderableType.Sprite;
                if (Reference is AnimatedSprite)
                    m_CachedType = RenderableType.AnimatedSprite;
                if (Reference is Text)
                    m_CachedType = RenderableType.Text;
                return m_CachedType;
            }
        }

        IRenderable2D m_Reference = null;
        public IRenderable2D Reference
        {
            get { return m_Reference; }
            set
            {
                m_Reference = value;
                m_CachedType = RenderableType.Unknown;
            }
        }
    }
}
