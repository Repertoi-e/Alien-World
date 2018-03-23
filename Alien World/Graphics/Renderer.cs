using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alien_World.Graphics
{
    public enum RendererBufferType
    {
        Color = 0b01,
        Depth = 0b10
    }

    public class Renderer
    {
        static Renderer s_Instance;
        public static Renderer Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new Renderer();
                return s_Instance;
            }
        }

        static List<BlendState> m_BlendStates = new List<BlendState>();
        static List<DepthStencilState> m_DepthStencilStates = new List<DepthStencilState>();

        Context m_Context = null;

        private Renderer() { }

        public void Init()
        {
            if (!(m_Context is null))
                throw new InvalidOperationException("renderer already initialized");

            m_Context = Context.Instance;

            CreateBlendStates();
            CreateDepthStencilStates();

            SetBlend(false);
            SetDepthTesting(true);

            SharpDX.DXGI.Device dxgiDev = m_Context.Dev.QueryInterface<SharpDX.DXGI.Device>();
            SharpDX.DXGI.AdapterDescription adapterDesc = dxgiDev.Adapter.Description;

            Console.WriteLine("----------------------------------");
            Console.WriteLine(" Direct3D 11:");
            Console.WriteLine("    " + adapterDesc.Description);
            Console.WriteLine("    VRAM: " + adapterDesc.DedicatedVideoMemory / 1024 / 1024 + " MB");
            Console.WriteLine("----------------------------------");

            dxgiDev.Dispose();
        }

        public void Clear(RendererBufferType buffer)
        {
            if ((buffer & RendererBufferType.Color) != 0)
                Context.Instance.DevCon.ClearRenderTargetView(Context.Instance.BackBuffer, Color.CornflowerBlue);
            if ((buffer & RendererBufferType.Depth) != 0)
                Context.Instance.DevCon.ClearDepthStencilView(Context.Instance.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public void Present()
        {
            Context.Instance.Present();
        }

        void CreateBlendStates()
        {
            {
                var desc = new BlendStateDescription();

                desc.RenderTarget[0].IsBlendEnabled = false;
                desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

                m_BlendStates.Add(new BlendState(m_Context.Dev, desc));
            }
            {
                var desc = new BlendStateDescription
                {
                    AlphaToCoverageEnable = false,
                    IndependentBlendEnable = false
                };

                desc.RenderTarget[0].IsBlendEnabled = true;
                desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
                desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
                desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

                m_BlendStates.Add(new BlendState(m_Context.Dev, desc));
            }
        }

        void CreateDepthStencilStates()
        {
            {
                var desc = new DepthStencilStateDescription
                {
                    IsDepthEnabled = true,
                    DepthComparison = Comparison.Less,
                    DepthWriteMask = DepthWriteMask.All,
                    IsStencilEnabled = false,
                    StencilReadMask = 0xff,
                    StencilWriteMask = 0xff,
                    FrontFace = new DepthStencilOperationDescription
                    {
                        Comparison = Comparison.Always,
                        PassOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment
                    },
                    BackFace = new DepthStencilOperationDescription
                    {
                        Comparison = Comparison.Always,
                        PassOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement
                    }
                };
                m_DepthStencilStates.Add(new DepthStencilState(m_Context.Dev, desc));
            }
            {
                var desc = new DepthStencilStateDescription
                {
                    IsDepthEnabled = false,
                    DepthWriteMask = DepthWriteMask.Zero,
                    DepthComparison = Comparison.Always,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xff,
                    StencilWriteMask = 0xff,
                    FrontFace = new DepthStencilOperationDescription
                    {
                        Comparison = Comparison.Always,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation = StencilOperation.Increment
                    },
                    BackFace = new DepthStencilOperationDescription
                    {
                        Comparison = Comparison.Never,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation = StencilOperation.Keep
                    }
                };
                m_DepthStencilStates.Add(new DepthStencilState(m_Context.Dev, desc));
            }
        }

        public void SetWireframe(bool enabled)
        {
            m_Context.SetWireframe(enabled);
        }

        public void SetDepthTesting(bool enabled)
        {
            m_Context.DevCon.OutputMerger.SetDepthStencilState(m_DepthStencilStates[enabled ? 0 : 1]);
        }

        public void SetBlend(bool enabled)
        {
            m_Context.DevCon.OutputMerger.SetBlendState(m_BlendStates[enabled ? 1 : 0]);
        }
    }
}
