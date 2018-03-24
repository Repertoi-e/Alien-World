using System;

using SharpDX;

namespace Alien_World.Graphics.Buffers
{
    using D3D11 = SharpDX.Direct3D11;

    public enum BufferUsage : uint
    {
        STATIC = D3D11.ResourceUsage.Dynamic,
        DYNAMIC = D3D11.ResourceUsage.Dynamic
    }

    public class VertexBuffer : IDisposable
    {
        D3D11.BufferDescription m_Desc;
        D3D11.Buffer m_Handle;
        D3D11.InputLayout m_InputLayout;
        int m_Size, m_Stride;
        BufferUsage m_Usage;

        bool m_Disposed = false;

        public VertexBuffer(BufferUsage usage = BufferUsage.STATIC)
        {
            m_Usage = usage;

            m_Desc = new D3D11.BufferDescription
            {
                Usage = (D3D11.ResourceUsage)m_Usage,
                BindFlags = D3D11.BindFlags.VertexBuffer,
                CpuAccessFlags = D3D11.CpuAccessFlags.Write
            };
        }

        ~VertexBuffer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool managed)
        {
            if (!m_Disposed)
            {
                m_Handle.Dispose();
                m_InputLayout.Dispose();
                m_Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DataBox Map(D3D11.MapMode mapMode = D3D11.MapMode.WriteDiscard)
        {
            return Context.Instance.DevCon.MapSubresource(m_Handle, 0, mapMode, D3D11.MapFlags.None);
        }

        public void Unmap()
        {
            Context.Instance.DevCon.UnmapSubresource(m_Handle, 0);
        }

        public void SetData<T>(T data, int size) where T : struct
        {
            if (size > m_Size)
                Resize(size);

            DataBox ptr = Map();
            Utilities.Write(ptr.DataPointer, ref data);
            Unmap();
        }

        public void SetData<T>(T[] data, int size) where T : struct
        {
            if (size > m_Size)
                Resize(size);

            DataBox ptr = Map();
            Utilities.Write(ptr.DataPointer, data, 0, data.Length);
            Unmap();
        }

        public void SetLayout(BufferLayout layout)
        {
            m_Stride = layout.Size;

            var elements = layout.Elements;
            D3D11.InputElement[] elementsDesc = new D3D11.InputElement[layout.Elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                BufferLayout.Element element = elements[i];
                elementsDesc[i] = new D3D11.InputElement(element.Name, 0, element.Type, 
                    element.Offset, 0, D3D11.InputClassification.PerVertexData, 0);
            }
            m_InputLayout = new D3D11.InputLayout(Context.Instance.Dev, Shaders.Shader.CurrentlyBound.Data.VS.Data, elementsDesc);
        }

        public void Resize(int size)
        {
            m_Handle?.Dispose();
            m_Size = size;
            m_Desc.SizeInBytes = m_Size;
            m_Handle = new SharpDX.Direct3D11.Buffer(Context.Instance.Dev, m_Desc);
        }

        public void Bind()
        {
            Context.Instance.DevCon.InputAssembler.InputLayout = m_InputLayout;
            D3D11.VertexBufferBinding binding = new D3D11.VertexBufferBinding
            {
                Buffer = m_Handle,
                Stride = m_Stride,
                Offset = 0
            };
            Context.Instance.DevCon.InputAssembler.SetVertexBuffers(0, binding);
        }
    }
}
