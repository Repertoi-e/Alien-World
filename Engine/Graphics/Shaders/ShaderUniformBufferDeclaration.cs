using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Shaders
{
    public class ShaderUniformBufferDeclaration
    {
        public static readonly int BufferAlignment = 16;

        public String Name { get; }
        public List<ShaderUniformDeclaration> Uniforms { get; } = new List<ShaderUniformDeclaration>();
        public int Register { get; }
        public int Size { get; private set; }
        public ShaderType ShaderType { get; }

        public ShaderUniformBufferDeclaration(string name, int register, ShaderType shaderType)
        {
            Name = name;
            Register = register;
            ShaderType = shaderType;
            Size = 0;
        }

        public void PushUniform(ShaderUniformDeclaration decl)
        {
            int offset = 0;

            if (Uniforms.Count > 0)
            {
                ShaderUniformDeclaration previous = Uniforms.Last();
                offset = previous.Offset + previous.Size;
            }
            decl.Offset = offset;
            Size += decl.Size;
            Uniforms.Add(decl);
        }

        public void Align()
        {
            Size = (Size + (BufferAlignment - 1)) / BufferAlignment * BufferAlignment;
        }

        public ShaderUniformDeclaration FindUniform(string name)
	    {
            foreach (ShaderUniformDeclaration decl in Uniforms)
                if (decl.Name.Equals(name))
                    return decl;
	    	return null;
	    }
    }
}
