using SharpDX;

namespace Engine.Entity_Component_System.Components
{
    public class PositionComponent : IComponent
    {
        public Vector2 Value;
        public float X { get { return Value.X; } set { Value.X = value; } }
        public float Y { get { return Value.Y; } set { Value.Y = value; } }

        public PositionComponent() { Value = new Vector2(); }

        public PositionComponent(float x, float y)
        {
            Value = new Vector2(x, y);
        }

        public IComponent Clone() => (IComponent)MemberwiseClone();
        public void Dispose() { }
    }

    public class PositionComponentJson : ComponentJsonDefinition<PositionComponent>
    {
        public float[] Start { get; set; }

        public override PositionComponent GetComponentFromDefinition(Entity entity) => new PositionComponent(Start[0], Start[1]);
    }
}
