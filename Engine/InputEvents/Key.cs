using System;

using WinApi.User32;

namespace Engine.Input_Events
{
    public struct KeyModifiers
    {
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public bool Control { get; set; }
    }

    public class KeyPressedEventArgs : EventArgs
    {
        public VirtualKey KeyCode { get; set; }
        public KeyModifiers Modifiers { get; set; }
        public bool Repeat { get; set; }
    }
    public delegate void KeyPressedEventHandler(KeyPressedEventArgs e);

    public class KeyReleasedEventArgs : EventArgs
    {
        public VirtualKey KeyCode { get; set; }
        public KeyModifiers Modifiers { get; set; }
    }
    public delegate void KeyReleasedEventHandler(KeyReleasedEventArgs e);

    public class KeyTypedEventArgs : EventArgs
    {
        public char CodePoint { get; set; }
    }
    public delegate void KeyTypedEventHandler(KeyTypedEventArgs e);
}
