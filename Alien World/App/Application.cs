using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using WinApi.Gdi32;
using WinApi.Kernel32;
using WinApi.User32;
using WinApi.Windows;

using log4net;

using Alien_World.Graphics;

namespace Alien_World.App
{
    public class ApplicationInfo
    {
        public string Title;
        public int Width, Height;
        public bool FullScreen, VSync;
        public bool Running = true;
    }

    public class Application
    {
        static Application s_Instance;
        public static Application Instance { get
            {
                if (s_Instance == null)
                    s_Instance = new Application();
                return s_Instance;
            } }

        public static readonly ILog Logger = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ApplicationInfo m_Info = null;
        public ApplicationInfo Info { get { return m_Info; } }

        public List<Layer> Layers { get; } = new List<Layer>();

        IntPtr m_hWnd;
        WindowProc m_WindowProc;

        private Application() { }

        public void Init(string title, int width, int height, bool vsync, bool fullscreen)
        {
            if (m_Info != null)
                throw new InvalidOperationException("application already initialized");
            m_Info = new ApplicationInfo
            {
                Title = title,
                Width = width,
                Height = height,
                VSync = vsync,
                FullScreen = fullscreen
            };

            IntPtr hInstance = Kernel32Methods.GetModuleHandle(IntPtr.Zero);

            m_WindowProc = WindowProc;
            var wc = new WindowClassEx
            {
                Size = (uint)Marshal.SizeOf<WindowClassEx>(),
                ClassName = "MainWindow",
                CursorHandle = User32Helpers.LoadCursor(IntPtr.Zero, SystemCursor.IDC_ARROW),
                IconHandle = User32Helpers.LoadIcon(IntPtr.Zero, SystemIcon.IDI_APPLICATION),
                Styles = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW | WindowClassStyles.CS_OWNDC,
                BackgroundBrushHandle = new IntPtr((int)StockObject.WHITE_BRUSH),
                WindowProc = m_WindowProc,
                InstanceHandle = hInstance
            };

            if (User32Methods.RegisterClassEx(ref wc) == 0)
                throw new ExternalException("window registration failed");

            NetCoreEx.Geometry.Rectangle size = new NetCoreEx.Geometry.Rectangle(0, 0, width, height);
            User32Methods.AdjustWindowRectEx(ref size, WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS,
                false, WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_WINDOWEDGE);

            m_hWnd = User32Methods.CreateWindowEx(WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_WINDOWEDGE, wc.ClassName, 
                title, WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS,
                (int)CreateWindowFlags.CW_USEDEFAULT, (int)CreateWindowFlags.CW_USEDEFAULT,
                size.Right + (-size.Left), size.Bottom + (-size.Top), IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);

            if (m_hWnd == IntPtr.Zero)
                throw new ExternalException("window creation failed");

            User32Methods.ShowWindow(m_hWnd, ShowWindowCommands.SW_SHOWNORMAL);
            User32Methods.UpdateWindow(m_hWnd);

            Context.Instance.Init(m_hWnd, m_Info);
            Script.LuaEngine.Instance.Init();
        }

        private static IntPtr WindowProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr result = IntPtr.Zero;

            var msg = (WM)umsg;
            switch (msg)
            {
                case WM.ERASEBKGND:
                    return new IntPtr(1);
                case WM.CLOSE: User32Methods.PostQuitMessage(0);
                        break;
                case WM.PAINT:
                    {
                        var hdc = User32Methods.BeginPaint(hwnd, out PaintStruct ps);
                        User32Methods.FillRect(hdc, ref ps.PaintRect,
                            Gdi32Helpers.GetStockObject(StockObject.WHITE_BRUSH));
                        User32Methods.EndPaint(hwnd, ref ps);
                        break;
                    }
                case WM.KEYDOWN: InputManager.Instance.KeyPressed((VirtualKey)wParam.ToInt32(), lParam.ToInt32() & 0x40000000);
                        break;
                case WM.KEYUP: InputManager.Instance.KeyReleased((VirtualKey)wParam.ToInt32());
                        break;
                case WM.CHAR: InputManager.Instance.KeyTyped((char)wParam.ToInt32());
                    break;
                case WM.MOUSEMOVE:
                    {
                        int x = unchecked((short)(long)lParam);
                        int y = unchecked((short)((long)lParam >> 16));
                        InputManager.Instance.MouseMoved(x, y);
                        break;
                    }
                case WM.LBUTTONDOWN:
                    {
                        int x = unchecked((short)(long)lParam);
                        int y = unchecked((short)((long)lParam >> 16));
                        InputManager.Instance.MousePressed(MouseButton.Left, x, y);
                        break;
                    }
                case WM.LBUTTONUP:
                    {
                        int x = unchecked((short)(long)lParam);
                        int y = unchecked((short)((long)lParam >> 16));
                        InputManager.Instance.MouseReleased(MouseButton.Left, x, y);
                        break;
                    }
                case WM.RBUTTONDOWN:
                    {
                        int x = unchecked((short)(long)lParam);
                        int y = unchecked((short)((long)lParam >> 16));
                        InputManager.Instance.MousePressed(MouseButton.Right, x, y);
                        break;
                    }
                case WM.RBUTTONUP:
                    {
                        int x = unchecked((short)(long)lParam);
                        int y = unchecked((short)((long)lParam >> 16));
                        InputManager.Instance.MouseReleased(MouseButton.Right, x, y);
                        break;
                    }
                default:
                    result = User32Methods.DefWindowProc(hwnd, umsg, wParam, lParam);
                    break;
            }
            return result;
        }

        public void OnRender()
        {
            Context.Instance.Clear(RendererBufferType.Color | RendererBufferType.Depth);

            foreach (Layer layer in Layers)
                layer._Render();

            Context.Instance.Present();
        }

        public void OnUpdate()
        {
            foreach (Layer layer in Layers)
                layer._Update();
        }

        public void OnTick()
        {
            User32Methods.SetWindowText(m_hWnd, $"{m_Info.Title} | {Time.FPS} fps, {Time.UPS} ups");
        }

        public int Run()
        {
            int result = new GameLoop(m_Info, OnRender, OnUpdate, OnTick).Run();
            Context.Instance.Dispose();
            foreach (Layer layer in Layers)
                layer.Dispose();
            return result;
        }
    }
}
