using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Alien_World.Collections
{
    public struct BitVector64
    {
        long m_Data;

        public struct Section
        {
            private short m_Mask;
            private short m_Offset;

            internal Section(short mask, short offset)
            {
                m_Mask = mask;
                m_Offset = offset;
            }

            public short Mask => m_Mask;
            public short Offset => m_Offset;

            public override bool Equals(object o)
            {
                if (!(o is Section))
                    return false;

                Section section = (Section)o;
                return m_Mask == section.m_Mask && m_Offset == section.m_Offset;
            }

            public override int GetHashCode()
            {
                return (m_Mask.GetHashCode() << 16) + m_Offset.GetHashCode();
            }

            public override string ToString()
            {
                return $"Section{{0x{Convert.ToString(m_Mask, 16)}, 0x{Convert.ToString(m_Offset, 16)}}}";
            }

            public static string ToString(Section value)
            {
                return $"Section{{0x{Convert.ToString(value.Mask, 16)}, 0x{Convert.ToString(value.Offset, 16)}}}";
            }

            public static bool operator ==(Section v1, Section v2) { return v1.m_Mask == v2.m_Mask && v1.m_Offset == v2.m_Offset; }
            public static bool operator !=(Section v1, Section v2) { return v1.m_Mask != v2.m_Mask && v1.m_Offset != v2.m_Offset; }
            public bool Equals(Section obj) { return m_Mask == obj.m_Mask && m_Offset == obj.m_Offset; }
        }

        public long Data => m_Data;

        public BitVector64(int init) => m_Data = init;
        public BitVector64(long source) => m_Data = source;
        public BitVector64(BitVector32 source) => m_Data = source.Data;
        public BitVector64(BitVector64 source) => m_Data = source.m_Data;

        public long this[Section section]
        {
            get { return ((m_Data >> section.Offset) & section.Mask); }

            set
            {
                if (value < 0)
                    throw new ArgumentException("Section can't hold negative values");
                if (value > section.Mask)
                    throw new ArgumentException("Value too large to fit in section");
                m_Data &= ~(section.Mask << section.Offset);
                m_Data |= (value << section.Offset);
            }
        }

        public bool this[long mask]
        {
            get { return (m_Data & mask) == mask; }

            set
            {
                if (value)
                    m_Data |= mask;
                else
                    m_Data &= ~mask;
            }
        }

        public void Reset()
        {
            m_Data = 0;
        }

        public int[] GetSetBits()
        {
            List<int> result = new List<int>();
            for (int bit = 0; bit < 64; bit++)
            {
                int mask = 1 << bit;
                if ((m_Data & mask) != 0)
                    result.Add(bit);
            }
            return result.ToArray();
        }

        public static long CreateMask()
        {
            return CreateMask(0);   // 1;
        }

        public static long CreateMask(long prev)
        {
            if (prev == 0)
                return 1;
            if (prev == Int64.MinValue)
                throw new InvalidOperationException("All bits set");
            return prev << 1;
        }

        public static Section CreateSection(int maxValue)
        {
            return CreateSection(maxValue, new Section(0, 0));
        }

        public static Section CreateSection(int maxValue, BitVector64.Section previous)
        {
            if (maxValue < 1)
                throw new ArgumentException("maxValue");

            int bit = HighestSetBit(maxValue) + 1;
            int mask = (1 << bit) - 1;
            int offset = previous.Offset + NumberOfSetBits(previous.Mask);

            if (offset > 64)
                throw new ArgumentException("Sections cannot exceed 64 bits in total");

            return new Section((short)mask, (short)offset);
        }

        public override bool Equals(object o)
        {
            if (!(o is BitVector64))
                return false;

            return m_Data == ((BitVector64)o).m_Data;
        }

        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(this);
        }

        public static string ToString(BitVector64 value)
        {
            StringBuilder sb = new StringBuilder(0x2d);
            sb.Append("BitVector64{");
            ulong data = (ulong)value.Data;
            for (int i = 0; i < 0x40; i++)
            {
                sb.Append(((data & 0x8000000000000000) == 0) ? '0' : '1');
                data = data << 1;
            }

            sb.Append("}");
            return sb.ToString();
        }

        public static bool operator ==(long one, BitVector64 other) { return one == other.Data; }
        public static bool operator ==(BitVector64 one, BitVector64 other) { return one.Data == other.Data; }
        public static bool operator !=(long one, BitVector64 other) { return one != other.Data; }
        public static bool operator !=(BitVector64 one, BitVector64 other) { return one.Data != other.Data; }

        public static long operator &(BitVector64 one, long other) { return one.Data & other; }
        public static long operator &(BitVector64 one, BitVector64 other) { return one & other.Data; }

        private static int NumberOfSetBits(int i)
        {
            int count = 0;
            for (int bit = 0; bit < 64; bit++)
            {
                int mask = 1 << bit;
                if ((i & mask) != 0)
                    count++;
            }
            return count;
        }

        private static int HighestSetBit(int i)
        {
            for (int bit = 63; bit >= 0; bit--)
            {
                int mask = 1 << bit;
                if ((mask & i) != 0)
                    return bit;
            }
            return -1;
        }
    }
}
