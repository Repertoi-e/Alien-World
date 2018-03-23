using Entitas;

using SharpDX;

public struct PositionComponent : IComponent
{
    public float X, Y;

    public static implicit operator Vector2(PositionComponent comp) { return new Vector2(comp.X, comp.Y); }
}
