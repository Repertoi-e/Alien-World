using System;

using SharpDX;
using SharpDX.Direct3D11;

namespace Alien_World.Graphics.Buffers
{
    public enum BufferUsage : uint
    {
        STATIC = ResourceUsage.Dynamic,
        DYNAMIC = ResourceUsage.Dynamic
    }

    public class VertexBuffer : IDisposable
    {
        BufferDescription m_Desc;
        SharpDX.Direct3D11.Buffer m_Handle;
        InputLayout m_InputLayout;
        int m_Size, m_Stride;
        BufferUsage m_Usage;

        bool m_Disposed = false;

        public VertexBuffer(BufferUsage usage = BufferUsage.STATIC)
        {
            m_Usage = usage;

            m_Desc = new BufferDescription
            {
                Usage = (ResourceUsage)m_Usage,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write
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

        public DataBox Map(MapMode mapMode = MapMode.WriteDiscard)
        {
            return Context.Instance.DevCon.MapSubresource(m_Handle, 0, mapMode, MapFlags.None);
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
            InputElement[] elementsDesc = new InputElement[layout.Elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                BufferLayout.Element element = elements[i];
                elementsDesc[i] = new InputElement(element.Name, 0, element.Type, element.Offset, 0, InputClassification.PerVertexData, 0);
            }
            m_InputLayout = new InputLayout(Context.Instance.Dev, Shaders.Shader.CurrentlyBound.Data.VS.Data, elementsDesc);
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
            VertexBufferBinding binding = new VertexBufferBinding
            {
                Buffer = m_Handle,
                Offset = 0,
                Stride = m_Stride
            };
            Context.Instance.DevCon.InputAssembler.SetVertexBuffers(0, binding);
        }
    }
}
