using System;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Alien_World.Graphics
{
    using DirectWrite = SharpDX.DirectWrite;

    public class Text : IRenderable2D, IDisposable
    {
        public string String { get; set; }
        public DirectWrite.TextFormat TextFormat { get; set; }
        public DirectWrite.TextLayout TextLayout { get; set; }
        float m_Width, m_Height;
        private bool m_Disposed = false;

        Brush brush = new SolidColorBrush(Context.Instance.D2DRenderTarget, new RawColor4(0, 0, 0, 1));

        public Text(string text, string fontFamily, float fontSize, float width, float height)
        {
            String = text;
            TextFormat = new DirectWrite.TextFormat(Context.Instance.DWriteFactory, fontFamily, fontSize);

            m_Width = width;
            m_Height = height;

            UpdateTextLayout();
        }

        public void UpdateTextLayout()
        {
            TextLayout = new DirectWrite.TextLayout(Context.Instance.DWriteFactory, String, TextFormat, m_Width, m_Height);
        }

        public void Render(Vector2 position, IRenderer2D renderer)
        {
            Context context = Context.Instance;
            context.D2DRenderTarget.BeginDraw();

            context.D2DRenderTarget.DrawTextLayout(position, TextLayout, brush);
            context.D2DRenderTarget.EndDraw();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                }

                ((IDisposable)brush).Dispose();

                m_Disposed = true;
            }
        }
        
        ~Text()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
