using System;
using System.Collections.Generic;

using Alien_World.Resource_Manager;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System.Diagnostics;

namespace Alien_World.Graphics
{
    public enum TextureFilter : uint
    {
        LINEAR = Filter.MinMagMipLinear,
        NEAREST = Filter.MinMagMipPoint
    }

    public class TextureHandle : IDisposable
    {
        public Texture2DDescription TextureDesc;
        public Texture2D Texture;
        public ShaderResourceView ResourceView;
        public SamplerState SamplerState;
        public SamplerStateDescription SamplerDesc;

        public void Dispose()
        {
            ((IDisposable)Texture).Dispose();
            ((IDisposable)ResourceView).Dispose();
            ((IDisposable)SamplerState).Dispose();
        }
    };

    public class Texture : Resource_Manager.Resource, IEquatable<Texture>
    {
        TextureHandle m_Handle = new TextureHandle();
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Size { get { return new Vector2(Width, Height); } }
        public Format PixelFormat { get; private set; }
        public TextureFilter TextureFilter { get; }

        public Texture(string name, string filePath, BitmapSource bitmapSource, TextureFilter filter)
        {
            Name = name;
            FilePath = filePath;
            TextureFilter = filter;
            PixelFormat = Format.R8G8B8A8_UNorm;

            Load(bitmapSource);
        }

        protected override void DisposeUnmanaged()
        {
            if (!m_Disposed)
            {
                m_Handle.Dispose();
                m_Disposed = true;
            }
        }

        private void Load(BitmapSource bitmapSource)
        {
            Width = bitmapSource.Size.Width;
            Height = bitmapSource.Size.Height;

            int mipLevels = 1;
            {
                int width = Width, height = Height;
                while (width > 1 && height > 1)
                {
                    width = Math.Max(width / 2, 1);
                    height = Math.Max(height / 2, 1);
                    mipLevels++;
                }
            }

            FormatSupport fmtSupport = Context.Instance.Dev.CheckFormatSupport(PixelFormat);
            Debug.Assert((fmtSupport & FormatSupport.MipAutogen) != 0);

            m_Handle.TextureDesc = new Texture2DDescription
            {
                Width = Width,
                Height = Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = PixelFormat,
                CpuAccessFlags = CpuAccessFlags.None,
                Usage = ResourceUsage.Default,
                // BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                BindFlags = BindFlags.ShaderResource,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription
                {
                    Count = 1,
                    Quality = 0
                },
            };

            DataStream data = new DataStream(Width * Height * 4, true, true);
            bitmapSource.CopyPixels(Width * 4, data);

            var dataRectangle = new DataRectangle(data.DataPointer, Width * 4);
            m_Handle.Texture = new Texture2D(Context.Instance.Dev, m_Handle.TextureDesc, dataRectangle);

            var srvDesc = new ShaderResourceViewDescription
            {
                Format = m_Handle.TextureDesc.Format,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = m_Handle.TextureDesc.MipLevels
                }
            };
            m_Handle.ResourceView = new ShaderResourceView(Context.Instance.Dev, m_Handle.Texture, srvDesc);

            // Context.Instance.DevCon.UpdateSubresource(ref dataRectangle, m_Handle.Texture);
            // Context.Instance.DevCon.GenerateMips(m_Handle.ResourceView);

            m_Handle.SamplerDesc = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MinimumLod = 0,
                MaximumLod = 3.402823466e+38f,
                Filter = (Filter)TextureFilter,
                ComparisonFunction = Comparison.Never
            };
            m_Handle.SamplerState = new SamplerState(Context.Instance.Dev, m_Handle.SamplerDesc);
        }

        public void Bind(int slot = 0)
        {
            Context.Instance.DevCon.PixelShader.SetShaderResource(slot, m_Handle.ResourceView);
            Context.Instance.DevCon.PixelShader.SetSampler(slot, m_Handle.SamplerState);
        }

        public void Unbind(int slot = 0)
        {
            Context.Instance.DevCon.PixelShader.SetShaderResource(slot, null);
        }

        public override bool Equals(object obj)
        {
            Texture texture = obj as Texture;
            return texture != null && this == texture;
        }

        public bool Equals(Texture other)
        {
            return m_Handle == other.m_Handle;
        }

        public override int GetHashCode()
        {
            return m_Handle.GetHashCode();
        }

        public static bool operator ==(Texture one, Texture other) { return one.m_Handle == other.m_Handle; }
        public static bool operator !=(Texture one, Texture other) { return one.m_Handle != other.m_Handle; }
    }

    public struct TextureRegion
    {
        public Texture Texture;
        public Vector2[] UVs;
    }

    public class TextureSheet
    {
        Texture m_Texture = null;
        List<TextureRegion> m_Regions = new List<TextureRegion>();

        public TextureSheet(string filePath, TextureFilter filtering, int rows, int columns)
            : this(ResourceLoader.LoadTexture(filePath, filtering), rows, columns)
        {
        }

        public TextureSheet(Texture texture, int rows, int columns)
        {
            m_Texture = texture;

            float normalX = m_Texture.Width / columns; // width of each piece
            float normalY = m_Texture.Height / rows;   // height of each piece

            for (int xa = 0; xa < columns; xa++)
                for (int ya = 0; ya < rows; ya++)
                {
                    float u0 = xa * normalX / m_Texture.Width;
                    float v0 = ya * normalY / m_Texture.Height;
                    float u1 = (xa + 1) * normalX / m_Texture.Width;
                    float v1 = (ya + 1) * normalY / m_Texture.Height;

                    m_Regions.Add(new TextureRegion
                    {
                        Texture = m_Texture,
                        UVs = new Vector2[]
                        {
                            new Vector2(u0, v0),
                            new Vector2(u1, v0),
                            new Vector2(u1, v1),
                            new Vector2(u0, v1)
                        }
                    });
                }
        }

        public Texture Texture { get { return m_Texture; } }
        public List<TextureRegion> Regions { get { return m_Regions; } }
    }
}
