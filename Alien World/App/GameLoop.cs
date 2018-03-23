using System;
using System.Diagnostics;

using WinApi.User32;
using WinApi.Windows;

namespace Alien_World.App
{
    public class GameLoop : EventLoopCore
    {
        public delegate void Callback();

        ApplicationInfo m_Info;
        Callback m_UpdateCallback, m_RenderCallback, m_TickCallback;

        public GameLoop(ApplicationInfo info, Callback render, Callback update, Callback tick) : base(null)
        {
            m_Info = info;
            m_UpdateCallback = update;
            m_RenderCallback = render;
            m_TickCallback = tick;
        }

        public override int RunCore()
        {
            Stopwatch timer = Stopwatch.StartNew();
            int updates = 0, frames = 0;
            const double tick = 1000.0f / 60.0f;
            double then = 0, elapsedSeconds = 0, dt = 0;
            for (; m_Info.Running; frames++)
            {
                while (User32Helpers.PeekMessage(out Message msg, IntPtr.Zero, 0, 0, PeekMessageFlags.PM_REMOVE))
                {
                    if (msg.Value == (uint)WM.QUIT)
                        m_Info.Running = false;
                    User32Methods.TranslateMessage(ref msg);
                    User32Methods.DispatchMessage(ref msg);
                }

                double now = timer.Elapsed.TotalMilliseconds;
                if ((now - then) > tick)
                {
                    m_UpdateCallback();
                    Time.DeltaTime = (float)(now - dt);
                    dt = now;
                    then = now;
                    updates++;
                }
                {
                    Stopwatch frameTimer = Stopwatch.StartNew();
                    m_RenderCallback();
                    Time.FrameTime = (float)frameTimer.Elapsed.TotalMilliseconds;
                }
                if (timer.Elapsed.TotalSeconds - elapsedSeconds > 1.0f)
                {
                    m_TickCallback();
                    Time.FPS = frames;
                    Time.UPS = updates;
                    frames = 0;
                    updates = 0;
                    elapsedSeconds++;
                }
            }
            return 0;
        }
    }
}
