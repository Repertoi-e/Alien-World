using System;
using System.Collections.Generic;

using Alien_World.App;

namespace Alien_World.Graphics
{
    using DX = SharpDX;
    using D3D = SharpDX.Direct3D;
    using D2D1 = SharpDX.Direct2D1;
    using D3D11 = SharpDX.Direct3D11;
    using DXGI = SharpDX.DXGI;
    using DirectWrite = SharpDX.DirectWrite;

    public enum RendererBufferType
    {
        Color = 0b01,
        Depth = 0b10
    }

    public class Context : IDisposable
    {
        static Context s_Instance;
        public static Context Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new Context();
                return s_Instance;
            }
        }

        public D3D11.Device Dev { get; private set; }
        public D3D11.DeviceContext DevCon { get; private set; }
        public DXGI.SwapChain SwapChain { get; private set; }
        public D3D11.RenderTargetView BackBuffer { get { return m_RenderTargetView; } }
        public D3D11.DepthStencilView DepthStencilView { get { return m_DepthStencilView; } }
        public D2D1.Factory D2DFactory { get; private set; }
        public D2D1.RenderTarget D2DRenderTarget { get; private set; }
        public DirectWrite.Factory DWriteFactory;

        D3D.FeatureLevel m_FeatureLevel = D3D.FeatureLevel.Level_11_1;
        int m_MSAAQuality;

        D3D11.RenderTargetView m_RenderTargetView;
        D3D11.DepthStencilView m_DepthStencilView;
        D3D11.DepthStencilState m_DepthStencilState;
        DX.Viewport m_ScreenViewport;
        D3D11.Texture2D m_Backbuffer, m_DepthBuffer;

        List<D3D11.BlendState> m_BlendStates = new List<D3D11.BlendState>();
        List<D3D11.DepthStencilState> m_DepthStencilStates = new List<D3D11.DepthStencilState>();
        List<D3D11.RasterizerState> m_RasterizerStates = new List<D3D11.RasterizerState>();

        ApplicationInfo m_ApplicationInfo = null;

        private Context() { }

        public void Init(IntPtr hWnd, ApplicationInfo appInfo)
        {
            if (m_ApplicationInfo != null)
                throw new InvalidOperationException("context already initialized");

            m_ApplicationInfo = appInfo;

            var swapChainDesc = new DXGI.SwapChainDescription
            {
                ModeDescription = new DXGI.ModeDescription
                {
                    Width = m_ApplicationInfo.Width,
                    Height = m_ApplicationInfo.Height,
                    RefreshRate = new DXGI.Rational(60, 1),
                    Format = DXGI.Format.R8G8B8A8_UNorm
                },
                SampleDescription = new DXGI.SampleDescription
                {
                    Count = 4
                },
                BufferCount = 1,
                Usage = DXGI.Usage.RenderTargetOutput,
                OutputHandle = hWnd,
                IsWindowed = !m_ApplicationInfo.FullScreen,
                SwapEffect = DXGI.SwapEffect.Discard,
                Flags = DXGI.SwapChainFlags.AllowModeSwitch
            };
            D3D11.Device.CreateWithSwapChain(D3D.DriverType.Hardware,
                D3D11.DeviceCreationFlags.BgraSupport | D3D11.DeviceCreationFlags.Debug, new[] { m_FeatureLevel },
                swapChainDesc, out D3D11.Device dev, out DXGI.SwapChain swapChain);
            Dev = dev;
            DevCon = Dev.ImmediateContext;
            SwapChain = swapChain;

            D2DFactory = new D2D1.Factory(D2D1.FactoryType.SingleThreaded, D2D1.DebugLevel.Information);
            DWriteFactory = new DirectWrite.Factory(DirectWrite.FactoryType.Shared);

            m_MSAAQuality = Dev.CheckMultisampleQualityLevels(DXGI.Format.R8G8B8A8_UNorm, 4);

            D3D11.DeviceDebug d3dDebug = dev.QueryInterface<D3D11.DeviceDebug>();
            if (d3dDebug != null)
            {
                D3D11.InfoQueue infoQueue = dev.QueryInterface<D3D11.InfoQueue>();
                if (infoQueue != null)
                {
                    D3D11.MessageId[] hide =
                    {
                        D3D11.MessageId.MessageIdDeviceDrawSamplerNotSet
                    };
                    infoQueue.AddStorageFilterEntries(new D3D11.InfoQueueFilter() {
                        DenyList = new D3D11.InfoQueueFilterDescription { Ids = hide } });
                }
            }

            Resize();

            CreateBlendStates();
            CreateDepthStencilStates();
            CreateRasterizerStates();

            SetBlend(false);
            SetDepthTesting(true);
            SetWireframe(false);

            SharpDX.DXGI.Device dxgiDev = Dev.QueryInterface<SharpDX.DXGI.Device>();
            SharpDX.DXGI.AdapterDescription adapterDesc = dxgiDev.Adapter.Description;

            Console.WriteLine("----------------------------------");
            Console.WriteLine(" Direct3D 11:");
            Console.WriteLine("    " + adapterDesc.Description);
            Console.WriteLine("    VRAM: " + adapterDesc.DedicatedVideoMemory / 1024 / 1024 + " MB");
            Console.WriteLine("----------------------------------");

            dxgiDev.Dispose();
        }

        public void Resize()
        {
            m_RenderTargetView?.Dispose();
            m_DepthStencilView?.Dispose();
            m_DepthBuffer?.Dispose();
            m_Backbuffer?.Dispose();
            D2DRenderTarget?.Dispose();

            SwapChain.ResizeBuffers(1, m_ApplicationInfo.Width, m_ApplicationInfo.Height, DXGI.Format.R8G8B8A8_UNorm, DXGI.SwapChainFlags.None);

            m_Backbuffer = SwapChain.GetBackBuffer<D3D11.Texture2D>(0);
            m_RenderTargetView = new D3D11.RenderTargetView(Dev, m_Backbuffer);

            var d2dRenderTargetProps = new D2D1.RenderTargetProperties
            {
                PixelFormat = new D2D1.PixelFormat(DXGI.Format.Unknown, D2D1.AlphaMode.Premultiplied),
                Type = D2D1.RenderTargetType.Default,
                DpiX = D2DFactory.DesktopDpi.Width,
                DpiY = D2DFactory.DesktopDpi.Height
            };

            DXGI.Surface2 backBuffer = SwapChain.GetBackBuffer<DXGI.Surface2>(0);
            D2DRenderTarget = new D2D1.RenderTarget(D2DFactory, backBuffer, d2dRenderTargetProps);
            backBuffer.Dispose();

            var depthBufferDesc = new D3D11.Texture2DDescription
            {
                Format = DXGI.Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = m_Backbuffer.Description.Width,
                Height = m_Backbuffer.Description.Height,
                SampleDescription = SwapChain.Description.SampleDescription,
                BindFlags = D3D11.BindFlags.DepthStencil,
            };
            m_Backbuffer.Dispose();

            var depthStencilViewDesc = new D3D11.DepthStencilViewDescription
            {
                Dimension = SwapChain.Description.SampleDescription.Count > 1 || SwapChain.Description.SampleDescription.Quality > 0
                            ? D3D11.DepthStencilViewDimension.Texture2DMultisampled
                            : D3D11.DepthStencilViewDimension.Texture2D
            };

            var depthStencilStateDesc = new D3D11.DepthStencilStateDescription
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

            m_DepthBuffer = new D3D11.Texture2D(Dev, depthBufferDesc);
            m_DepthStencilView = new D3D11.DepthStencilView(Dev, m_DepthBuffer, depthStencilViewDesc);
            m_DepthStencilState = new D3D11.DepthStencilState(Dev, depthStencilStateDesc);

            SetRenderTargets(m_DepthStencilView, m_RenderTargetView);
            DevCon.OutputMerger.DepthStencilState = m_DepthStencilState;

            m_ScreenViewport.X = 0;
            m_ScreenViewport.Y = 0;
            m_ScreenViewport.Width = m_ApplicationInfo.Width;
            m_ScreenViewport.Height = m_ApplicationInfo.Height;
            m_ScreenViewport.MinDepth = 0.0f;
            m_ScreenViewport.MaxDepth = 1.0f;
            DevCon.Rasterizer.SetViewport(m_ScreenViewport);
        }

        public void Clear(RendererBufferType buffer)
        {
            if ((buffer & RendererBufferType.Color) != 0)
                DevCon.ClearRenderTargetView(BackBuffer, DX.Color.CornflowerBlue);
            if ((buffer & RendererBufferType.Depth) != 0)
                DevCon.ClearDepthStencilView(DepthStencilView,
                    D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        void CreateBlendStates()
        {
            {
                var desc = new D3D11.BlendStateDescription();

                desc.RenderTarget[0].IsBlendEnabled = false;
                desc.RenderTarget[0].RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All;

                m_BlendStates.Add(new D3D11.BlendState(Dev, desc));
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

                m_BlendStates.Add(new D3D11.BlendState(Dev, desc));
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
                m_DepthStencilStates.Add(new D3D11.DepthStencilState(Dev, desc));
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
                m_DepthStencilStates.Add(new D3D11.DepthStencilState(Dev, desc));
            }
        }

        void CreateRasterizerStates()
        {
            {
                var desc = D3D11.RasterizerStateDescription.Default();
                desc.IsFrontCounterClockwise = false;
                m_RasterizerStates.Add(new D3D11.RasterizerState(Dev, desc));
            }
            {
                var desc = D3D11.RasterizerStateDescription.Default();
                desc.IsFrontCounterClockwise = false;
                desc.FillMode = D3D11.FillMode.Wireframe;
                m_RasterizerStates.Add(new D3D11.RasterizerState(Dev, desc));
            }
        }

        public void SetWireframe(bool enabled)
        {
            DevCon.Rasterizer.State = m_RasterizerStates[enabled ? 1 : 0];
        }

        public void SetDepthTesting(bool enabled)
        {
            DevCon.OutputMerger.SetDepthStencilState(m_DepthStencilStates[enabled ? 0 : 1]);
        }

        public void SetBlend(bool enabled)
        {
            DevCon.OutputMerger.SetBlendState(m_BlendStates[enabled ? 1 : 0]);
        }

        public void SetRenderTargets(D3D11.DepthStencilView depthStencilView, D3D11.RenderTargetView renderTargetView)
        {
            DevCon.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);
        }

        public void Present()
        {
            SwapChain.Present(m_ApplicationInfo.VSync ? 1 : 0, 0);
        }

        public void Dispose()
        {
            ((IDisposable)DevCon).Dispose();
            ((IDisposable)Dev).Dispose();
            ((IDisposable)D2DFactory).Dispose();
            ((IDisposable)D2DRenderTarget)?.Dispose();
            ((IDisposable)m_RenderTargetView)?.Dispose();
            ((IDisposable)m_DepthStencilView)?.Dispose();
            ((IDisposable)m_DepthBuffer)?.Dispose();
            ((IDisposable)m_Backbuffer)?.Dispose();
            ((IDisposable)D2DRenderTarget)?.Dispose();
        }
    }
}