using System;
using System.Reflection;

namespace Alien_World.Graphics.Shaders
{
    public class UniformTypeDescription : Attribute
    {
        public string Text { get; }
        public int Size { get; }

        public UniformTypeDescription(string text, int size)
        {
            Text = text;
            Size = size;
        }
    }

    public enum ShaderUniformType
    {
        NONE = 0,
        [UniformTypeDescription("float", sizeof(float))]
        FLOAT,
        [UniformTypeDescription("int32", sizeof(int))]
        INT,
        [UniformTypeDescription("float2", sizeof(float))]
        VEC2,
        [UniformTypeDescription("float3", sizeof(float) * 2)]
        VEC3,
        [UniformTypeDescription("float4", sizeof(float) * 3)]
        VEC4,
        [UniformTypeDescription("float4x4", sizeof(float) * 4 * 4)]
        MAT4,
        STRUCT
    }

    public static class ShaderUniformTypeMethods
    {
        public static UniformTypeDescription GetDescription(this ShaderUniformType en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(UniformTypeDescription), false);
                if (attrs != null && attrs.Length > 0)
                    return ((UniformTypeDescription)attrs[0]);
            }
            throw new ArgumentException(string.Format("Uniform type {0} doesn't have a description.", en.ToString()));
        }

        public static ShaderUniformType StringToUniformType(string str)
        {
            ShaderUniformType[] values = (ShaderUniformType[])Enum.GetValues(typeof(ShaderUniformType));
            foreach (ShaderUniformType value in values)
            {
                UniformTypeDescription desc;
                try
                {
                    desc = value.GetDescription();
                }
                catch (ArgumentException)
                {
                    continue;
                }
                if (desc.Text == str)
                    return value;
            }
            return ShaderUniformType.NONE;
        }
    }
}
