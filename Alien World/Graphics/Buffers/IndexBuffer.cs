using System;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Alien_World.Graphics.Buffers
{
    public class IndexBuffer : IDisposable
    {
        SharpDX.Direct3D11.Buffer m_Handle;
        public int Count { get; }
        bool m_Disposed = false;

        public unsafe IndexBuffer(uint[] data)
        {
            Count = data.Length;
            var desc = new BufferDescription
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = sizeof(uint) * Count,
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            DataStream dataStream = new DataStream(desc.SizeInBytes, true, true);
            foreach (uint i in data)
                dataStream.Write(i);
            dataStream.Position = 0;

            m_Handle = new SharpDX.Direct3D11.Buffer(Context.Instance.Dev, dataStream, desc);
        }

        ~IndexBuffer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool managed)
        {
            if (!m_Disposed)
            {
                if (managed)
                { }

                m_Handle.Dispose();

                m_Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Bind()
        {
            Context.Instance.DevCon.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Context.Instance.DevCon.InputAssembler.SetIndexBuffer(m_Handle, SharpDX.DXGI.Format.R32_UInt, 0);
        }
    }
}
