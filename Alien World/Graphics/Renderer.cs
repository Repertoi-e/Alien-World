using System;
using System.Collections.Generic;

using SharpDX;

namespace Alien_World.Graphics
{
    using D3D11 = SharpDX.Direct3D11;

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

        static List<D3D11.BlendState> m_BlendStates = new List<D3D11.BlendState>();
        static List<D3D11.DepthStencilState> m_DepthStencilStates = new List<D3D11.DepthStencilState>();
        static List<D3D11.RasterizerState> m_RasterizerStates = new List<D3D11.RasterizerState>();

        Context m_Context = null;

        private Renderer() { }

        public void Init()
        {
            if (!(m_Context is null))
                throw new InvalidOperationException("renderer already initialized");

            m_Context = Context.Instance;

            CreateBlendStates();
            CreateDepthStencilStates();
            CreateRasterizerStates();

            SetBlend(false);
            SetDepthTesting(true);
            SetWireframe(false);

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
                Context.Instance.DevCon.ClearDepthStencilView(Context.Instance.DepthStencilView,
                    D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        public void Present()
        {
            Context.Instance.Present();
        }

        void CreateBlendStates()
        {
            {
                var desc = new D3D11.BlendStateDescription();

                desc.RenderTarget[0].IsBlendEnabled = false;
                desc.RenderTarget[0].RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All;

                m_BlendStates.Add(new D3D11.BlendState(m_Context.Dev, desc));
            }
            {
                var desc = new D3D11.BlendStateDescription
                {
                    AlphaToCoverageEnable = false,
                    IndependentBlendEnable = false
                };

                desc.RenderTarget[0].IsBlendEnabled = true;
                desc.RenderTarget[0].SourceBlend = D3D11.BlendOption.SourceAlpha;
                desc.RenderTarget[0].DestinationBlend = D3D11.BlendOption.InverseSourceAlpha;
                desc.RenderTarget[0].BlendOperation = D3D11.BlendOperation.Add;
                desc.RenderTarget[0].SourceAlphaBlend = D3D11.BlendOption.SourceAlpha;
                desc.RenderTarget[0].DestinationAlphaBlend = D3D11.BlendOption.InverseSourceAlpha;
                desc.RenderTarget[0].AlphaBlendOperation = D3D11.BlendOperation.Add;
                desc.RenderTarget[0].RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All;

                m_BlendStates.Add(new D3D11.BlendState(m_Context.Dev, desc));
            }
        }

        void CreateDepthStencilStates()
        {
            {
                var desc = new D3D11.DepthStencilStateDescription
                {
                    IsDepthEnabled = true,
                    DepthComparison = D3D11.Comparison.Less,
                    DepthWriteMask = D3D11.DepthWriteMask.All,
                    IsStencilEnabled = false,
                    StencilReadMask = 0xff,
                    StencilWriteMask = 0xff,
                    FrontFace = new D3D11.DepthStencilOperationDescription
                    {
                        Comparison = D3D11.Comparison.Always,
                        PassOperation = D3D11.StencilOperation.Keep,
                        FailOperation = D3D11.StencilOperation.Keep,
                        DepthFailOperation = D3D11.StencilOperation.Increment
                    },
                    BackFace = new D3D11.DepthStencilOperationDescription
                    {
                        Comparison = D3D11.Comparison.Always,
                        PassOperation = D3D11.StencilOperation.Keep,
                        FailOperation = D3D11.StencilOperation.Keep,
                        DepthFailOperation = D3D11.StencilOperation.Decrement
                    }
                };
                m_DepthStencilStates.Add(new D3D11.DepthStencilState(m_Context.Dev, desc));
            }
            {
                var desc = new D3D11.DepthStencilStateDescription
                {
                    IsDepthEnabled = false,
                    DepthWriteMask = D3D11.DepthWriteMask.Zero,
                    DepthComparison = D3D11.Comparison.Always,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xff,
                    StencilWriteMask = 0xff,
                    FrontFace = new D3D11.DepthStencilOperationDescription
                    {
                        Comparison = D3D11.Comparison.Always,
                        FailOperation = D3D11.StencilOperation.Keep,
                        DepthFailOperation = D3D11.StencilOperation.Keep,
                        PassOperation = D3D11.StencilOperation.Increment
                    },
                    BackFace = new D3D11.DepthStencilOperationDescription
                    {
                        Comparison = D3D11.Comparison.Never,
                        FailOperation = D3D11.StencilOperation.Keep,
                        DepthFailOperation = D3D11.StencilOperation.Keep,
                        PassOperation = D3D11.StencilOperation.Keep
                    }
                };
                m_DepthStencilStates.Add(new D3D11.DepthStencilState(m_Context.Dev, desc));
            }
        }

        void CreateRasterizerStates()
        {
            {
                var desc = D3D11.RasterizerStateDescription.Default();
                desc.IsFrontCounterClockwise = false;
                m_RasterizerStates.Add(new D3D11.RasterizerState(m_Context.Dev, desc));
            }
            {
                var desc = D3D11.RasterizerStateDescription.Default();
                desc.IsFrontCounterClockwise = false;
                desc.FillMode = D3D11.FillMode.Wireframe;
                m_RasterizerStates.Add(new D3D11.RasterizerState(m_Context.Dev, desc));
            }
        }

        public void SetWireframe(bool enabled)
        {
            m_Context.DevCon.Rasterizer.State = m_RasterizerStates[enabled ? 1 : 0];
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
