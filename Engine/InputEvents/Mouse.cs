using System;

using WinApi.Windows;

namespace Engine.Input_Events
{
    public class MouseMovedEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public delegate void MouseMovedEventHandler(MouseMovedEventArgs e);

    public class MouseButtonPressedEventArgs : EventArgs
    {
        public MouseButton Button { get; set; }
        public KeyModifiers Modifiers;
        public int X { get; set; }
        public int Y { get; set; }
    }
    public delegate void MouseButtonPressedEventHandler(MouseButtonPressedEventArgs e);

    public class MouseButtonReleasedEventArgs : EventArgs
    {
        public MouseButton Button { get; set; }
        public KeyModifiers Modifiers;
        public int X { get; set; }
        public int Y { get; set; }
    }
    public delegate void MouseButtonReleasedEventHandler(MouseButtonReleasedEventArgs e);
}
