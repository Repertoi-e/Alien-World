using System;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using Engine.App;

namespace Engine.Graphics
{
    public class Context
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

        public Device Dev { get; private set; }
        public DeviceContext DevCon { get; private set; }
        public SharpDX.DXGI.SwapChain SwapChain { get; private set; }
        public RenderTargetView BackBuffer { get { return m_RenderTargetView; } }
        public DepthStencilView DepthStencilView { get { return m_DepthStencilView; } }

        FeatureLevel m_FeatureLevel = FeatureLevel.Level_11_0;
        int m_MSAAQuality;
        bool m_MSAAEnabled = false;
        bool m_DebugLayerEnabled = true;

        RenderTargetView m_RenderTargetView;
        DepthStencilView m_DepthStencilView;
        DepthStencilState m_DepthStencilState;
        Viewport m_ScreenViewport;
        Texture2D m_Backbuffer, m_DepthBuffer;

        DeviceDebug m_DebugLayer;

        ApplicationInfo m_ApplicationInfo = null;

        private Context() { }

        public void Init(IntPtr hWnd, ApplicationInfo appInfo)
        {
            if (m_ApplicationInfo != null)
                throw new InvalidOperationException("context already initialized");

            m_ApplicationInfo = appInfo;

            var swapChainDesc = new SharpDX.DXGI.SwapChainDescription
            {
                ModeDescription = new SharpDX.DXGI.ModeDescription
                {
                    Width = m_ApplicationInfo.Width,
                    Height = m_ApplicationInfo.Height,
                    RefreshRate = new SharpDX.DXGI.Rational(60, 1),
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm
                },
                SampleDescription = new SharpDX.DXGI.SampleDescription
                {
                    Count = 4
                },
                BufferCount = 1,
                Usage = SharpDX.DXGI.Usage.RenderTargetOutput,
                OutputHandle = hWnd,
                IsWindowed = !m_ApplicationInfo.FullScreen,
                SwapEffect = SharpDX.DXGI.SwapEffect.Discard,
                Flags = SharpDX.DXGI.SwapChainFlags.AllowModeSwitch
            };
            Device.CreateWithSwapChain(DriverType.Hardware,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug, new[] { m_FeatureLevel },
                swapChainDesc, out Device dev, out SharpDX.DXGI.SwapChain swapChain);
            Dev = dev;
            DevCon = Dev.ImmediateContext;
            SwapChain = swapChain;

            m_MSAAQuality = Dev.CheckMultisampleQualityLevels(SharpDX.DXGI.Format.R8G8B8A8_UNorm, 4);

            Resize();
        }

        public void Resize()
        {
            m_RenderTargetView?.Dispose();
            m_DepthStencilView?.Dispose();
            m_DepthBuffer?.Dispose();
            m_Backbuffer?.Dispose();

            SwapChain.ResizeBuffers(1, m_ApplicationInfo.Width, m_ApplicationInfo.Height, SharpDX.DXGI.Format.R8G8B8A8_UNorm, SharpDX.DXGI.SwapChainFlags.None);

            m_Backbuffer = SwapChain.GetBackBuffer<Texture2D>(0);
            m_RenderTargetView = new RenderTargetView(Dev, m_Backbuffer);
            
            var depthBufferDesc = new Texture2DDescription
            {
                Format = SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = m_Backbuffer.Description.Width,
                Height = m_Backbuffer.Description.Height,
                SampleDescription = SwapChain.Description.SampleDescription,
                BindFlags = BindFlags.DepthStencil,
            };
            m_Backbuffer.Dispose();

            var depthStencilViewDesc = new DepthStencilViewDescription
            {
                Dimension = SwapChain.Description.SampleDescription.Count > 1 || SwapChain.Description.SampleDescription.Quality > 0
                            ? DepthStencilViewDimension.Texture2DMultisampled
                            : DepthStencilViewDimension.Texture2D
            };

            var depthStencilStateDesc = new DepthStencilStateDescription
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

            m_DepthBuffer = new Texture2D(Dev, depthBufferDesc);
            m_DepthStencilView = new DepthStencilView(Dev, m_DepthBuffer, depthStencilViewDesc);
            m_DepthStencilState = new DepthStencilState(Dev, depthStencilStateDesc);

            SetRenderTargets(m_DepthStencilView, m_RenderTargetView);
            DevCon.OutputMerger.DepthStencilState = m_DepthStencilState;

            m_ScreenViewport.X = 0;
            m_ScreenViewport.Y = 0;
            m_ScreenViewport.Width = m_ApplicationInfo.Width;
            m_ScreenViewport.Height = m_ApplicationInfo.Height;
            m_ScreenViewport.MinDepth = 0.0f;
            m_ScreenViewport.MaxDepth = 1.0f;
            DevCon.Rasterizer.SetViewport(m_ScreenViewport);

            var rasterizerStateDesc = RasterizerStateDescription.Default();
            rasterizerStateDesc.IsFrontCounterClockwise = false;

            RasterizerState rs = new RasterizerState(Dev, rasterizerStateDesc);
            DevCon.Rasterizer.State = rs;
        }

        public void SetRenderTargets(DepthStencilView depthStencilView, RenderTargetView renderTargetView)
        {
            DevCon.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);
        }

        public void Present()
        {
            SwapChain.Present(m_ApplicationInfo.VSync ? 1 : 0, 0);
        }
    }
}