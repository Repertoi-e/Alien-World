using System;
using System.Reflection;

namespace Engine.Graphics.Shaders
{
    public class ResourceTypeDescription : Attribute
    {
        public string Text { get; }

        public ResourceTypeDescription(string text)
        {
            Text = text;
        }
    }

    public enum ShaderResourceType
    {
        NONE = 0,
        [ResourceTypeDescription("Texture2D")]
        TEXTURE_2D,
        [ResourceTypeDescription("TextureCube")]
        TEXTURE_CUBE,
        [ResourceTypeDescription("SamplerState")]
        SAMPLER_STATE
    }

    public static class ShaderResourceTypeMethods
    {
        public static ResourceTypeDescription GetDescription(this ShaderResourceType en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(ResourceTypeDescription), false);
                if (attrs != null && attrs.Length > 0)
                    return ((ResourceTypeDescription)attrs[0]);
            }
            throw new ArgumentException(string.Format("Resource type {0} doesn't have a description.", en.ToString()));
        }

        public static ShaderResourceType StringToResourceType(string str)
        {
            ShaderResourceType[] values = (ShaderResourceType[])Enum.GetValues(typeof(ShaderResourceType));
            foreach (ShaderResourceType value in values)
            {
                ResourceTypeDescription desc;
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
            return ShaderResourceType.NONE;
        }

        public static bool IsStringResource(string str)
        {
            ShaderResourceType[] values = (ShaderResourceType[])Enum.GetValues(typeof(ShaderResourceType));
            foreach (ShaderResourceType value in values)
            {
                ResourceTypeDescription desc;
                try
                {
                    desc = value.GetDescription();
                }
                catch (ArgumentException)
                {
                    continue;
                }
                if (desc.Text == str)
                    return true;
            }
            return false;
        }
    }
}
