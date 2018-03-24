using System;

using SharpDX;

namespace Alien_World.Graphics.Buffers
{
    using D3D = SharpDX.Direct3D;
    using D3D11 = SharpDX.Direct3D11;

    public class IndexBuffer : IDisposable
    {
        D3D11.Buffer m_Handle;
        public int Count { get; }
        bool m_Disposed = false;

        public unsafe IndexBuffer(uint[] data)
        {
            Count = data.Length;
            var desc = new D3D11.BufferDescription
            {
                Usage = D3D11.ResourceUsage.Default,
                SizeInBytes = sizeof(uint) * Count,
                BindFlags = D3D11.BindFlags.IndexBuffer,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            };

            DataStream dataStream = new DataStream(desc.SizeInBytes, true, true);
            foreach (uint i in data)
                dataStream.Write(i);
            dataStream.Position = 0;

            m_Handle = new D3D11.Buffer(Context.Instance.Dev, dataStream, desc);
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
            Context.Instance.DevCon.InputAssembler.PrimitiveTopology = D3D.PrimitiveTopology.TriangleList;
            Context.Instance.DevCon.InputAssembler.SetIndexBuffer(m_Handle, SharpDX.DXGI.Format.R32_UInt, 0);
        }
    }
}
