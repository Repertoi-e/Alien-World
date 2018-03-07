using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.File_System
{
    public class FileSystemEntity : IEquatable<FileSystemEntity>
    {
        public IFileSystem FileSystem { get; private set; }
        public FileSystemPath Path { get; private set; }
        public string Name => Path.EntityName;

        public FileSystemEntity(IFileSystem fileSystem, FileSystemPath path)
        {
            FileSystem = fileSystem;
            Path = path;
        }

        public override bool Equals(object obj)
        {
            var other = obj as FileSystemEntity;
            return (other != null) && ((IEquatable<FileSystemEntity>)this).Equals(other);
        }

        public override int GetHashCode()
        {
            return FileSystem.GetHashCode() ^ Path.GetHashCode();
        }

        bool IEquatable<FileSystemEntity>.Equals(FileSystemEntity other)
        {
            return FileSystem.Equals(other.FileSystem) && Path.Equals(other.Path);
        }

        public static FileSystemEntity Create(IFileSystem fileSystem, FileSystemPath path)
        {
            if (path.IsFile)
                return new File(fileSystem, path);
            else
                return new Directory(fileSystem, path);
        }
    }

    public class File : FileSystemEntity, IEquatable<File>
    {
        public File(IFileSystem fileSystem, FileSystemPath path)
            : base(fileSystem, path)
        {
            if (!path.IsFile)
                throw new ArgumentException("the specified path is not a file");
        }

        public bool Equals(File other)
        {
            return ((IEquatable<FileSystemEntity>)this).Equals(other);
        }
    }

    public class Directory : FileSystemEntity, IEquatable<Directory>
    {
        public Directory(IFileSystem fileSystem, FileSystemPath path)
            : base(fileSystem, path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is no directory.", "path");
        }

        public bool Equals(Directory other)
        {
            return ((IEquatable<FileSystemEntity>)this).Equals(other);
        }
    }
}
