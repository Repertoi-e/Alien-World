using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Alien_World.App;
using Alien_World.Resource_Manager;
using Alien_World.Graphics.Buffers;
using Alien_World.Graphics.Shaders;

using SharpDX;

namespace Alien_World.Graphics
{
    public struct Camera
    {
        public Matrix ProjectionMatrix, ViewMatrix;
        public Vector3 Position, Rotation;
    }

    [StructLayout(LayoutKind.Explicit, Size = (2 + 2 + 1) * sizeof(float) + sizeof(uint))]
    struct Vertex
    {
        [FieldOffset(0)]
        public Vector2 Position;
        [FieldOffset(2 * sizeof(float))]
        public Vector2 UV;
        [FieldOffset(4 * sizeof(float))]
        public float TID;
        [FieldOffset(5 * sizeof(float))]
        public uint Color;
    }

    public unsafe class BatchRenderer2D : IRenderer2D, IDisposable
    {
        static readonly int MaxSprites = 5000;
        static readonly int IndicesSize = MaxSprites * 6;
        static readonly int MaxTextures = 31;

        [StructLayout(LayoutKind.Explicit, Size = 2 * 4 * 4 * sizeof(float))]
        internal struct R2DUniformMatrices
        {
            [FieldOffset(0)]
            public Matrix ProjectionMatrix;

            [FieldOffset(4 * 4 * sizeof(float))]
            public Matrix ViewMatrix;
        }
        R2DUniformMatrices m_R2DUniformMatricesBuffer = new R2DUniformMatrices();
        int m_R2DUniformMatricesBufferSlot;

        Shader m_Shader;

        VertexBuffer m_VertexBuffer;
        Vertex* m_Buffer;
        IndexBuffer m_IndexBuffer;
        Camera m_Camera;
        int m_IndexCount = 0;
        List<Texture> m_Textures = new List<Texture>(MaxTextures + 1);

        public BatchRenderer2D()
        {
            m_Shader = ResourceLoader.LoadShader("Renderer2D", "/Assets/Shaders/");
            m_Shader.Bind();

            foreach (ShaderUniformBufferDeclaration bufferDecl in m_Shader.VSSystemUniforms)
                if (bufferDecl.Name == "VSUniforms")
                    m_R2DUniformMatricesBufferSlot = bufferDecl.Register;

            BufferLayout layout = new BufferLayout();
            layout.Push<Vector2>("POSITION", 1);
            layout.Push<Vector2>("TEXCOORD", 1);
            layout.Push<float>("ID", 1);     
            layout.Push<byte>("COLOR", 4);

            m_VertexBuffer = new VertexBuffer();
            m_VertexBuffer.Resize(layout.Size * MaxSprites * 4);
            m_VertexBuffer.SetLayout(layout);

            uint[] indices = new uint[IndicesSize];

            uint offset = 0;
            for (uint i = 0; i < IndicesSize; i += 6)
            {
                indices[i] = offset;
                indices[i + 1] = offset + 1;
                indices[i + 2] = offset + 2;

                indices[i + 3] = offset + 2;
                indices[i + 4] = offset + 3;
                indices[i + 5] = offset;

                offset += 4;
            }
            m_IndexBuffer = new IndexBuffer(indices);

            Matrix.OrthoOffCenterLH(0, Application.Instance.Info.Width, Application.Instance.Info.Height,
                0, -1, 1, out Matrix ProjectionMatrix);
            Camera = new Camera
            {
                ProjectionMatrix = ProjectionMatrix,
                ViewMatrix = new Matrix(),
                Position = new Vector3(),
                Rotation = new Vector3()
            };
        }

        public void Dispose()
        {
            m_Shader.Dispose();
            m_VertexBuffer.Dispose();
            m_IndexBuffer.Dispose();
        }

        private float SubmitTexture(Texture texture)
        {
            int index = -1;
            for (int i = 0; i < m_Textures.Count; i++)
                if (m_Textures[i] == texture)
                {
                    index = i;
                    break;
                }
            if (index != -1)
                return index + 1.0f;
            else
            {
                if (m_Textures.Count > MaxTextures)
                {
                    Present();
                    Begin();
                }
                m_Textures.Add(texture);
                return m_Textures.Count;
            }
        }

        public void Begin()
        {
            m_VertexBuffer.Bind();
            m_Buffer = (Vertex*) m_VertexBuffer.Map().DataPointer.ToPointer();
        }

        public void Submit(Vector2 position, Sprite renderable)
        {
            Vector2 size = renderable.Size;
            Vector2[] uvs = renderable.UVs;
            Texture texture = renderable.Texture;
            uint color = renderable.Color;
            
            float tid = 0;
            if (!(texture is null))
                tid = SubmitTexture(texture);
            
            m_Buffer->Position = position;
            m_Buffer->UV = uvs[0];
            m_Buffer->TID = tid;
            m_Buffer->Color = color;
            m_Buffer++;

            m_Buffer->Position = new Vector2(position.X + size.X, position.Y);
            m_Buffer->UV = uvs[1];
            m_Buffer->TID = tid;
            m_Buffer->Color = color;
            m_Buffer++;
            
            m_Buffer->Position = position + size;
            m_Buffer->UV = uvs[2];
            m_Buffer->TID = tid;
            m_Buffer->Color = color;
            m_Buffer++;
            
            m_Buffer->Position = new Vector2(position.X, position.Y + size.Y);
            m_Buffer->UV = uvs[3];
            m_Buffer->TID = tid;
            m_Buffer->Color = color;
            m_Buffer++;
            
            m_IndexCount += 6;
        }

        public void DrawLine(float x0, float y0, float x1, float y1, uint color, float thickness)
        {
            Vector2 normal = new Vector2(y1 - y0, -(x1 - x0));
            normal.Normalize();
            normal *= thickness;

            m_Buffer->Position = new Vector2(x0 + normal.X, y0 + normal.Y);
            m_Buffer->UV = Vector2.Zero;                                
            m_Buffer->TID = 0;                                          
            m_Buffer->Color = color;                                    
            m_Buffer++;                                                 
                                                                        
            m_Buffer->Position = new Vector2(x1 + normal.X, y1 + normal.Y);
            m_Buffer->UV = Vector2.Zero;                                
            m_Buffer->TID = 0;                                          
            m_Buffer->Color = color;                                    
            m_Buffer++;                                                 
                                                                        
            m_Buffer->Position = new Vector2(x1 - normal.X, y1 - normal.Y);
            m_Buffer->UV = Vector2.Zero;                                
            m_Buffer->TID = 0;                                          
            m_Buffer->Color = color;                                    
            m_Buffer++;                                                 
                                                                        
            m_Buffer->Position = new Vector2(x0 - normal.X, y0 - normal.Y);
            m_Buffer->UV = Vector2.Zero;
            m_Buffer->TID = 0;
            m_Buffer->Color = color;
            m_Buffer++;

            m_IndexCount += 6;
        }

        public void DrawPolygon(List<Vector2> vertices, uint color, float thickness = 0.5f)
        {
            int size = vertices.Count;
            if (size < 3)
                return;
            for (int i = 0; i < size; i++)
            {
                Vector2 p1 = vertices[i];
                Vector2 p2 = vertices[(i + 1) % size];
                DrawLine(p1.X, p1.Y, p2.X, p2.Y, color, thickness);
            }
        }

        public void FillPolygon(List<Vector2> vertices, uint color)
        {
            int size = vertices.Count;
            if (size < 3)
                return;
            int indexSize = size - 1;

            int i = 1;
            while (i < indexSize)
            {
                m_Buffer->Position = vertices[0];
                m_Buffer->UV = Vector2.Zero;
                m_Buffer->TID = 0;
                m_Buffer->Color = color;
                m_Buffer++;

                m_Buffer->Position = vertices[i];
                m_Buffer->UV = Vector2.Zero;
                m_Buffer->TID = 0;
                m_Buffer->Color = color;
                m_Buffer++;
                i++;

                m_Buffer->Position = vertices[i > indexSize ? indexSize : i];
                m_Buffer->UV = Vector2.Zero;
                m_Buffer->TID = 0;
                m_Buffer->Color = color;
                m_Buffer++;
                i++;

                m_Buffer->Position = vertices[i > indexSize ? indexSize : i];
                m_Buffer->UV = Vector2.Zero;
                m_Buffer->TID = 0;
                m_Buffer->Color = color;
                m_Buffer++;

                m_IndexCount += 6;
            }
        }

        public void FillRect(float x, float y, float width, float height, uint color)
        {
            m_Buffer->Position = new Vector2(x, y);
            m_Buffer->UV = Vector2.Zero;
            m_Buffer->TID = 0;
            m_Buffer->Color = color;
            m_Buffer++;

            m_Buffer->Position = new Vector2(x + width, y);
            m_Buffer->UV = Vector2.Zero;
            m_Buffer->TID = 0;
            m_Buffer->Color = color;
            m_Buffer++;

            m_Buffer->Position = new Vector2(x + width, y + width);
            m_Buffer->UV = Vector2.Zero;
            m_Buffer->TID = 0;
            m_Buffer->Color = color;
            m_Buffer++;

            m_Buffer->Position = new Vector2(x, y + width);
            m_Buffer->UV = Vector2.Zero;
            m_Buffer->TID = 0;
            m_Buffer->Color = color;
            m_Buffer++;

            m_IndexCount += 6;
        }

        public void Present()
        {
            m_VertexBuffer.Unmap();

            Context.Instance.SetDepthTesting(false);
            Context.Instance.SetBlend(true);

            m_Shader.Bind();
            m_Shader.SetVSSystemUniformBuffer(m_R2DUniformMatricesBuffer, m_R2DUniformMatricesBufferSlot);

            for (int i = 0; i < m_Textures.Count; i++)
                m_Textures[i].Bind(i);

            m_VertexBuffer.Bind();
            m_IndexBuffer.Bind();
            Context.Instance.DevCon.DrawIndexed(m_IndexBuffer.Count, 0, 0);

            for (int i = 0; i < m_Textures.Count; i++)
                m_Textures[i].Unbind(i);

            m_Textures.Clear();
            m_IndexCount = 0;
        }

        public Camera Camera { get { return m_Camera; }
            set
            {
                m_Shader.Bind();
                m_Camera = value;

                m_Camera.ProjectionMatrix.Transpose();
                m_R2DUniformMatricesBuffer.ProjectionMatrix = m_Camera.ProjectionMatrix;
                
                // TODO:
                m_R2DUniformMatricesBuffer.ViewMatrix = Matrix.Identity;
            }
        }
    }
}
