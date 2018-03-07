using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Engine.Collections;

namespace Engine.File_System
{
    public class PhysicalFileSystem : IFileSystem
    {
        public string PhysicalRoot { get; private set; }

        public PhysicalFileSystem()
        {
            PhysicalRoot = System.IO.Directory.GetCurrentDirectory().ToString() + Path.DirectorySeparatorChar;
        }

        public string GetPhysicalPath(FileSystemPath path)
        {
            return Path.Combine(PhysicalRoot, path.ToString().Remove(0, 1).Replace(FileSystemPath.DirectorySeparator, Path.DirectorySeparatorChar));
        }

        public FileSystemPath GetVirtualFilePath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("the specified path is not member of the PhysicalRoot", "physicalPath");
            string virtualPath = FileSystemPath.DirectorySeparator + physicalPath.Remove(0, PhysicalRoot.Length).Replace(Path.DirectorySeparatorChar, FileSystemPath.DirectorySeparator);
            return FileSystemPath.Parse(virtualPath);
        }

        public FileSystemPath GetVirtualDirectoryPath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("the specified path is not member of the PhysicalRoot", "physicalPath");
            string virtualPath = FileSystemPath.DirectorySeparator + physicalPath.Remove(0, PhysicalRoot.Length).Replace(Path.DirectorySeparatorChar, FileSystemPath.DirectorySeparator);
            if (virtualPath[virtualPath.Length - 1] != FileSystemPath.DirectorySeparator)
                virtualPath += FileSystemPath.DirectorySeparator;
            return FileSystemPath.Parse(virtualPath);
        }

        public ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            string physicalPath = GetPhysicalPath(path);
            string[] dirs = System.IO.Directory.GetDirectories(physicalPath);
            string[] files = System.IO.Directory.GetFiles(physicalPath);
            IEnumerable<FileSystemPath> virtualDirs = dirs.Select(p => GetVirtualDirectoryPath(p));
            IEnumerable<FileSystemPath> virtualFiles = files.Select(p => GetVirtualFilePath(p));
            return new EnumerableCollection<FileSystemPath>(virtualDirs.Concat(virtualFiles), dirs.Length + files.Length);
        }

        public bool Exists(FileSystemPath path)
        {
            return path.IsFile ? System.IO.File.Exists(GetPhysicalPath(path)) : System.IO.Directory.Exists(GetPhysicalPath(path));
        }

        public Stream CreateFile(FileSystemPath path)
        {
            if (!path.IsFile)
                throw new ArgumentException("the specified path is not a file", "path");
            return System.IO.File.Create(GetPhysicalPath(path));
        }

        public Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            if (!path.IsFile)
                throw new ArgumentException("the specified path is not a file", "path");
            return System.IO.File.Open(GetPhysicalPath(path), FileMode.Open, access);
        }

        public void CreateDirectory(FileSystemPath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("the specified path is not a directory", "path");
            System.IO.Directory.CreateDirectory(GetPhysicalPath(path));
        }

        public void Delete(FileSystemPath path)
        {
            if (path.IsFile)
                System.IO.File.Delete(GetPhysicalPath(path));
            else
                System.IO.Directory.Delete(GetPhysicalPath(path), true);
        }

        public void Dispose()
        {
        }
    }
}
