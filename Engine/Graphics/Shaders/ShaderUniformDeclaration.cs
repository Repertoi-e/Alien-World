namespace Engine.Graphics.Shaders
{
    public class ShaderUniformDeclaration
    {
        public ShaderUniformType Type { get; }
        public string Name { get; }
        public int Size { get; }
        public int Count { get; }
        public int Location { get; }
        public ShaderStruct Struct { get; }

        int m_Offset;
        public int Offset
        {
            get { return m_Offset; }
            set
            {
                if (Type == ShaderUniformType.STRUCT)
                    Struct.Offset = value;
                m_Offset = value;
            }
        }

        public ShaderUniformDeclaration(ShaderUniformType type, string name, int count = 1)
        {
            Type = type;
            Name = name;
            Count = count;
            Size = type.GetDescription().Size * Count;
        }

        public ShaderUniformDeclaration(ShaderStruct uniformStruct, string name, int count = 1)
        {
            Type = ShaderUniformType.STRUCT;
            Struct = uniformStruct;
            Name = name;
            Count = count;
            Size = Struct.Size * Count;
        }
    }
}
