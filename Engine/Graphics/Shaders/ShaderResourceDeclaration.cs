namespace Engine.Graphics.Shaders
{
    public class ShaderResourceDeclaration
    {
        public ShaderResourceType Type { get; }
        public string Name { get; }
        public int Count { get; }
        public int Register { get; set; }

        public ShaderResourceDeclaration(ShaderResourceType type, string name, int count = 1)
        {
            Type = type;
            Name = name;
            Count = count;
        }
    }
}
