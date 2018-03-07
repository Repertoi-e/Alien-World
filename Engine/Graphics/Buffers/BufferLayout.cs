using System;
using System.Collections.Generic;

using SharpDX;

namespace Engine.Graphics.Buffers
{
    public class BufferLayout
    {
        public struct Element
        {
            public string Name;
            public SharpDX.DXGI.Format Type;
            public int TypeSize, Offset, Count;
        }

        List<Element> m_Elements = new List<Element>();
        int m_Size = 0;

        public void Push<T>(string name, int count)
        {
            if (typeof(T) == typeof(float))
                PushElement(name, SharpDX.DXGI.Format.R32_Float, sizeof(float), count);
            else if (typeof(T) == typeof(int))
                PushElement(name, SharpDX.DXGI.Format.R32_SInt, sizeof(int), count);
            else if (typeof(T) == typeof(uint))
                PushElement(name, SharpDX.DXGI.Format.R32_UInt, sizeof(uint), count);
            else if (typeof(T) == typeof(byte))
                PushElement(name, SharpDX.DXGI.Format.R8G8B8A8_UNorm, sizeof(byte), count);
            else if (typeof(T) == typeof(Vector2))
                PushElement(name, SharpDX.DXGI.Format.R32G32_Float, sizeof(float) * 2, count);
            else if (typeof(T) == typeof(Vector3))
                PushElement(name, SharpDX.DXGI.Format.R32G32B32_Float, sizeof(float) * 3, count);
            else if (typeof(T) == typeof(Vector4))
                PushElement(name, SharpDX.DXGI.Format.R32G32B32A32_Float, sizeof(float) * 4, count);
            else
                throw new NotImplementedException("Type not implemented.");
        }

        private void PushElement(string name, SharpDX.DXGI.Format type, int typeSize, int count)
        {
            m_Elements.Add(new Element
            {
                Name = name,
                Type = type,
                TypeSize = typeSize,
                Offset = m_Size,
                Count = count
            });
            m_Size += typeSize * count;
        }

        public List<Element> Elements { get { return m_Elements; } }
        public int Size { get { return m_Size; } }
    }
}
