using System.Runtime.InteropServices;

using Entitas;

using SharpDX;

public struct VelocityComponent : IComponent
{
    public Vector2 Velocity;

    public float X { get { return Velocity.X; } }
    public float Y { get { return Velocity.Y; } }

    public static implicit operator Vector2(VelocityComponent vel) { return vel.Velocity; }
}
