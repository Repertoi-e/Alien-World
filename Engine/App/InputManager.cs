using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinApi.Windows;
using WinApi.User32;

using Engine.Input_Events;
using NetCoreEx.Geometry;

namespace Engine.App
{
    public class InputManager
    {
        static InputManager s_Instance;
        public static InputManager Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new InputManager();
                return s_Instance;
            }
        }

        public event KeyPressedEventHandler KeyPressedHandler;
        public event KeyReleasedEventHandler KeyReleasedHandler;
        public event KeyTypedEventHandler KeyTypedHandler;
        public event MouseMovedEventHandler MouseMovedHandler;
        public event MouseButtonPressedEventHandler MouseButtonPressedHandler;
        public event MouseButtonReleasedEventHandler MouseButtonReleasedHandler;

        private InputManager() { }

        public void KeyPressed(VirtualKey keyCode, int repeatCount)
        {
            KeyPressedHandler?.Invoke(new KeyPressedEventArgs
            {
                KeyCode = keyCode,
                Modifiers = new KeyModifiers
                {
                    Alt = User32Methods.GetKeyState(VirtualKey.MENU).IsPressed,
                    Control = User32Methods.GetKeyState(VirtualKey.CONTROL).IsPressed,
                    Shift = User32Methods.GetKeyState(VirtualKey.SHIFT).IsPressed
                },
                Repeat = repeatCount != 0
            });
        }

        public void KeyReleased(VirtualKey keyCode)
        {
            KeyReleasedHandler?.Invoke(new KeyReleasedEventArgs
            {
                KeyCode = keyCode,
                Modifiers = new KeyModifiers
                {
                    Alt = User32Methods.GetKeyState(VirtualKey.MENU).IsPressed,
                    Control = User32Methods.GetKeyState(VirtualKey.CONTROL).IsPressed,
                    Shift = User32Methods.GetKeyState(VirtualKey.SHIFT).IsPressed
                }
            });
        }

        public void KeyTyped(char ch)
        {
            KeyTypedHandler?.Invoke(new KeyTypedEventArgs { CodePoint = ch });
        }

        public void MouseMoved(int x, int y)
        {
            MouseMovedHandler?.Invoke(new MouseMovedEventArgs { X = x, Y = y });
        }

        public void MousePressed(MouseButton button, int x, int y)
        {
            MouseButtonPressedHandler?.Invoke(new MouseButtonPressedEventArgs
            {
                Button = button,
                Modifiers = new KeyModifiers
                {
                    Alt = User32Methods.GetKeyState(VirtualKey.MENU).IsPressed,
                    Control = User32Methods.GetKeyState(VirtualKey.CONTROL).IsPressed,
                    Shift = User32Methods.GetKeyState(VirtualKey.SHIFT).IsPressed
                },
                X = x,
                Y = y
            });
        }

        public void MouseReleased(MouseButton button, int x, int y)
        {
            MouseButtonReleasedHandler?.Invoke(new MouseButtonReleasedEventArgs
            {
                Button = button,
                Modifiers = new KeyModifiers
                {
                    Alt = User32Methods.GetKeyState(VirtualKey.MENU).IsPressed,
                    Control = User32Methods.GetKeyState(VirtualKey.CONTROL).IsPressed,
                    Shift = User32Methods.GetKeyState(VirtualKey.SHIFT).IsPressed
                },
                X = x,
                Y = y
            });
        }
    }
}
