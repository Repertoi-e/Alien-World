using System.IO;

using Alien_World.Graphics;
using Alien_World.Graphics.Shaders;
using Alien_World.File_System;

using SharpDX.WIC;

namespace Alien_World.Resource_Manager
{
    public static class ResourceLoader
    {
        static readonly ImagingFactory Imgfactory = new ImagingFactory();
        static readonly PhysicalFileSystem m_FileSystem = new PhysicalFileSystem();
        public static PhysicalFileSystem FileSystem { get { return m_FileSystem; } }

        public static Texture LoadTexture(string path, TextureFilter filter, string name = null)
        {
            using (Stream stream = m_FileSystem.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
            {
                BitmapDecoder decoder = new BitmapDecoder(Imgfactory, stream, DecodeOptions.CacheOnDemand);
                BitmapFrameDecode frame = decoder.GetFrame(0);
                FormatConverter source = new FormatConverter(Imgfactory);

                source.Initialize(frame, PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null,
                    0.0, BitmapPaletteType.Custom);

                return new Texture(name ?? path, path, source, filter);
            }
        }

        public static Shader LoadShader(string name, string dir)
        {
            return new Shader(name, dir, LoadTextFile(dir + "Source/" + name + ".hlsl"));
        }

        public static string LoadTextFile(string path)
        {
            using (Stream stream = m_FileSystem.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
            {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
