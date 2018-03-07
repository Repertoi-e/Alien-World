using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using Engine.Resource_Manager;
using Engine.File_System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Engine.Graphics.Shaders
{
    public enum ShaderType
    {
        NONE = -1,
        VERTEX,
        PIXEL,

        END
    }

    public class Shader : Resource_Manager.Resource
    {
        public class ShaderData
        {
            public VertexShader VertexShader;
            public PixelShader PixelShader;
            public ShaderBytecode VS, PS;
        };

        public static Shader CurrentlyBound { get; set; } = null;

        public ShaderData Data { get; } = new ShaderData();
        List<ShaderUniformBufferDeclaration> m_VSUniformBuffers = new List<ShaderUniformBufferDeclaration>();
        List<ShaderUniformBufferDeclaration> m_PSUniformBuffers = new List<ShaderUniformBufferDeclaration>();
        ShaderUniformBufferDeclaration m_VSUserUniformBuffer = null, m_PSUserUniformBuffer = null;
        List<ShaderResourceDeclaration> m_Resources = new List<ShaderResourceDeclaration>();
        List<ShaderStruct> m_Structs = new List<ShaderStruct>();

        public List<ShaderUniformBufferDeclaration> VSSystemUniforms { get { return m_VSUniformBuffers; } }
        public List<ShaderUniformBufferDeclaration> PSSystemUniforms { get { return m_PSUniformBuffers; } }
        public ShaderUniformBufferDeclaration VSUserUniformBuffer { get { return m_VSUserUniformBuffer; } }
        public ShaderUniformBufferDeclaration PSUserUniformBuffer { get { return m_PSUserUniformBuffer; } }
        public List<ShaderResourceDeclaration> Resources { get { return m_Resources; } }

        SharpDX.Direct3D11.Buffer[] m_VSConstantBuffers;
        SharpDX.Direct3D11.Buffer[] m_PSConstantBuffers;

        public Shader(string name, string dir, string originalSource)
        {
            Name = name;
            FilePath = dir;
            Init(originalSource);
        }

        protected override void DisposeUnmanaged()
        {
            if (!m_Disposed)
            {
                Data.VertexShader.Dispose();
                Data.PixelShader.Dispose();
                Data.VS.Dispose();
                Data.PS.Dispose();
                m_Disposed = true;
            }
        }

        void Init(string originalSource)
        {
            Data.VS = GetBytecode(FilePath + Name + "VS.cso");
            Data.PS = GetBytecode(FilePath + Name + "PS.cso");
            Data.VertexShader = new VertexShader(Context.Instance.Dev, Data.VS);
            Data.PixelShader = new PixelShader(Context.Instance.Dev, Data.PS);

            Parse(RemoveComments(originalSource));
            CreateBuffers();
        }

        ShaderBytecode GetBytecode(string path)
        {
            using (Stream stream = ResourceLoader.FileSystem.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                return ShaderBytecode.FromStream(stream);
        }

        unsafe string RemoveComments(string source)
        {
            fixed (char* str = source)
            {
                String result = source;
                int startPos;
                while ((startPos = CString.FindStringPosition(result, "/*")) != -1)
                {
                    int endPos = CString.FindStringPosition(result, "*/");
                    Debug.Assert(endPos != -1);
                    result = result.Remove(startPos, endPos - startPos + 2);
                }

                while ((startPos = CString.FindStringPosition(result, "//")) != -1)
                {
                    int endPos = CString.FindStringPosition(result, "\n", startPos);
                    Debug.Assert(endPos != -1);
                    result = result.Remove(startPos, endPos - startPos + 1);
                }
                return result;
            }
        }

        unsafe void Parse(string source)
        {
            char* token;
            fixed (char* str = source)
                while ((token = CString.FindToken(str, "struct")) != null)
                    ParseStruct(CString.GetBlock(token, &str));

            fixed (char* str = source)
                while ((token = CString.FindToken(str, "cbuffer")) != null)
                    ParseCBuffer(CString.GetBlock(token, &str));

            fixed (char* str = source)
                while ((token = CString.FindToken(str, "Texture2D")) != null)
                    ParseTexture(CString.GetStatement(token, &str));

            fixed (char* str = source)
                while ((token = CString.FindToken(str, "TextureCube")) != null)
                    ParseTexture(CString.GetStatement(token, &str));
        }

        void ParseStruct(string block)
        {
            string[] tokens = block.Split(new[] { " ", "\t", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int index = 1;

            ShaderStruct uniformStruct = new ShaderStruct(tokens[index++]);
            index++; // {
            while (!tokens[index].Equals("}"))
            {
                string type = tokens[index++];
                string name = tokens[index++];

                if (type == ":")
                    continue;

                // Strip ; from name if present
                name = name.Replace(";", "");
                uniformStruct.AddField(new ShaderUniformDeclaration(ShaderUniformTypeMethods.StringToUniformType(type), name));
            }
            m_Structs.Add(uniformStruct);
        }

        void ParseCBuffer(string block)
        {
            string[] tokens = block.Split(new[] { " ", "\t", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int index = 1;

            string bufferName = tokens[index++];
            int reg = 0;
            if (tokens[index++] == ":") // Register specified
                reg = int.Parse(Regex.Match(tokens[index++], @"\d+").Value);

            ShaderUniformBufferDeclaration buffer = null;
            ShaderType shaderType = ShaderType.NONE;
            if (bufferName.StartsWith("VS"))
                shaderType = ShaderType.VERTEX;
            else if (bufferName.StartsWith("PS"))
                shaderType = ShaderType.PIXEL;
            else
                Debug.Assert(false, "CBuffer no shader type");

            index++; // {
            while (!tokens[index].Equals("}"))
            {
                string type = tokens[index++];
                string name = tokens[index++];

                // Strip ; from name if present
                name = name.Replace(";", "");

                if (buffer == null)
                {
                    buffer = new ShaderUniformBufferDeclaration(bufferName, reg, shaderType);
                    if (name.StartsWith("sys_"))
                    {
                        switch (shaderType)
                        {
                            case ShaderType.VERTEX: m_VSUniformBuffers.Add(buffer);
                                break;
                            case ShaderType.PIXEL: m_PSUniformBuffers.Add(buffer);
                                break;
                        }
                    }
                    else
                    {
                        switch (shaderType)
                        {
                            case ShaderType.VERTEX:
                                Debug.Assert(m_VSUserUniformBuffer == null);
                                m_VSUserUniformBuffer = buffer;
                                break;
                            case ShaderType.PIXEL:
                                Debug.Assert(m_PSUserUniformBuffer == null);
                                m_PSUserUniformBuffer = buffer;
                                break;
                        }
                    }
                }
                ShaderUniformType t = ShaderUniformTypeMethods.StringToUniformType(type);
                ShaderUniformDeclaration decl = null;
                if (t == ShaderUniformType.NONE)
                {
                    ShaderStruct s = FindStruct(type);
                    decl = new ShaderUniformDeclaration(s, name);
                }
                else
                {
                    decl = new ShaderUniformDeclaration(t, name);
                }
                buffer.PushUniform(decl);
            }
            buffer.Align();
        }

        void ParseTexture(string statement)
        {
            string[] tokens = statement.Split(new[] { " ", "\t", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int index = 0;
            int reg = 0;
            string type = tokens[index++];
            string name = tokens[index++];
            if (tokens[index++] == ":") // Register specified
                reg = int.Parse(Regex.Match(tokens[index++], @"\d+").Value);
            ShaderResourceDeclaration decl = new ShaderResourceDeclaration(ShaderResourceTypeMethods.StringToResourceType(type), name)
            {
                Register = reg
            };
            m_Resources.Add(decl);
        }

        ShaderStruct FindStruct(string type)
        {
            foreach (ShaderStruct s in m_Structs)
                if (s.Name == type)
                    return s;
            return null;
        }

        // Note: We only support a single user uniform buffer per shader
        void CreateBuffers()
        {
            m_VSConstantBuffers = new SharpDX.Direct3D11.Buffer[m_VSUniformBuffers.Count + (m_VSUserUniformBuffer != null ? 1 : 0)];

            foreach(ShaderUniformBufferDeclaration decl in m_VSUniformBuffers)
            {
                var desc = new BufferDescription
                {
                    SizeInBytes = decl.Size,
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write
                };
                m_VSConstantBuffers[decl.Register] = new SharpDX.Direct3D11.Buffer(Context.Instance.Dev, desc);
            }

            if (m_VSUserUniformBuffer != null)
            {
                ShaderUniformBufferDeclaration decl = m_VSUserUniformBuffer;

                var desc = new BufferDescription
                {
                    SizeInBytes = decl.Size,
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write
                };
                m_VSConstantBuffers[decl.Register] = new SharpDX.Direct3D11.Buffer(Context.Instance.Dev, desc);
            }

            m_PSConstantBuffers = new SharpDX.Direct3D11.Buffer[m_PSUniformBuffers.Count + (m_PSUserUniformBuffer != null ? 1 : 0)];

            foreach(ShaderUniformBufferDeclaration decl in m_PSUniformBuffers)
            {
                var desc = new BufferDescription
                {
                    SizeInBytes = decl.Size,
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write
                };
                m_PSConstantBuffers[decl.Register] = new SharpDX.Direct3D11.Buffer(Context.Instance.Dev, desc);
            }

            if (m_PSUserUniformBuffer != null)
            {
                ShaderUniformBufferDeclaration decl = m_PSUserUniformBuffer;

                var desc = new BufferDescription
                {
                    SizeInBytes = decl.Size,
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write
                };
                m_PSConstantBuffers[decl.Register] = new SharpDX.Direct3D11.Buffer(Context.Instance.Dev, desc);
            }
        }

        public void Bind()
        {
            CurrentlyBound = this;

            Context.Instance.DevCon.VertexShader.Set(Data.VertexShader);
            Context.Instance.DevCon.PixelShader.Set(Data.PixelShader);

            Context.Instance.DevCon.VertexShader.SetConstantBuffers(0, m_VSConstantBuffers);
            Context.Instance.DevCon.PixelShader.SetConstantBuffers(0, m_PSConstantBuffers);
        }

        public void SetVSSystemUniformBuffer<T>(T data, int slot) where T : struct
        {
            if (m_VSUserUniformBuffer != null)
                Debug.Assert(slot != m_VSUserUniformBuffer.Register);

            SharpDX.Direct3D11.Buffer cbuffer = m_VSConstantBuffers[slot];
            DataBox dataBox = Context.Instance.DevCon.MapSubresource(cbuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(dataBox.DataPointer, ref data);
            Context.Instance.DevCon.UnmapSubresource(cbuffer, 0);
        }

        public void SetPSSystemUniformBuffer<T>(T data, int slot) where T : struct
        {
            if (m_PSUserUniformBuffer != null)
                Debug.Assert(slot != m_PSUserUniformBuffer.Register);

            SharpDX.Direct3D11.Buffer cbuffer = m_PSConstantBuffers[slot];
            DataBox dataBox = Context.Instance.DevCon.MapSubresource(cbuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(dataBox.DataPointer, ref data);
            Context.Instance.DevCon.UnmapSubresource(cbuffer, 0);
        }

        public void SetVSUserUniformBuffer<T>(T data) where T : struct 
        {
            SharpDX.Direct3D11.Buffer cbuffer = m_VSConstantBuffers[m_VSUserUniformBuffer.Register];
            DataBox dataBox = Context.Instance.DevCon.MapSubresource(cbuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(dataBox.DataPointer, ref data);
            Context.Instance.DevCon.UnmapSubresource(cbuffer, 0);
        }

        public void SetPSUserUniformBuffer<T>(T data) where T : struct
        {
            SharpDX.Direct3D11.Buffer cbuffer = m_PSConstantBuffers[m_PSUserUniformBuffer.Register];
            DataBox dataBox = Context.Instance.DevCon.MapSubresource(cbuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(dataBox.DataPointer, ref data);
            Context.Instance.DevCon.UnmapSubresource(cbuffer, 0);
        }
    }
}
