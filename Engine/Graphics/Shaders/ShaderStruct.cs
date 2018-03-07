using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Shaders
{
    public class ShaderStruct
    {
        public string Name { get; }
        public List<ShaderUniformDeclaration> Fields { get; } = new List<ShaderUniformDeclaration>();
        public int Size { get; private set; } = 0;
        public int Offset { get; set; } = 0;

        public ShaderStruct(string name)
        {
            Name = name;
        }

        public void AddField(ShaderUniformDeclaration decl)
        {
            Size += decl.Size;
            int offset = 0;
            if (Fields.Count != 0)
            {
                ShaderUniformDeclaration previous = Fields.Last();
                offset = previous.Offset + previous.Size;
            }
            decl.Offset = offset;
            Fields.Add(decl);
        }
    }
}
