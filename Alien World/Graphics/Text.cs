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
        public DirectWrite.TextLayout TextLayout { get; set; }
        public DirectWrite.TextFormat TextFormat { get; set; }
        private bool m_Disposed = false;

        Brush brush = new SolidColorBrush(Context.Instance.D2DRenderTarget, new RawColor4(0, 0, 0, 1));

        public Text(string text, DirectWrite.TextFormat textFormat, float width, float height)
        {
            String = text;
            TextLayout = new DirectWrite.TextLayout(Context.Instance.DWriteFactory, text, textFormat, width, height);
            TextFormat = textFormat;
        }

        public void Render(Vector2 position, Renderer2D renderer)
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
