using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace Alien_World.File_System
{
    public struct FileSystemPath : IEquatable<FileSystemPath>, IComparable<FileSystemPath>
    {
        public static readonly char DirectorySeparator = '/';
        public static FileSystemPath Root = new FileSystemPath(DirectorySeparator.ToString());

        readonly string m_Path;

        public string Path => m_Path ?? "/";

        public bool IsDirectory => Path[Path.Length - 1] == DirectorySeparator;
        public bool IsFile => !IsDirectory;
        public bool IsRoot => Path.Length == 1;

        public string EntityName
        {
            get
            {
                string name = Path;
                if (IsRoot)
                    return null;
                int endOfName = name.Length;
                if (IsDirectory)
                    endOfName--;
                int startOfName = name.LastIndexOf(DirectorySeparator, endOfName - 1, endOfName) + 1;
                return name.Substring(startOfName, endOfName - startOfName);
            }
        }

        public FileSystemPath ParentPath
        {
            get
            {
                string parentPath = Path;
                if (IsRoot)
                    throw new InvalidOperationException("there is no parent of root");
                int lookaheadCount = parentPath.Length;
                if (IsDirectory)
                    lookaheadCount--;
                int index = parentPath.LastIndexOf(DirectorySeparator, lookaheadCount - 1, lookaheadCount);
                Debug.Assert(index >= 0);
                parentPath = parentPath.Remove(index + 1);
                return new FileSystemPath(parentPath);
            }
        }

        private FileSystemPath(string path)
        {
            m_Path = path;
        }

        public static bool IsRooted(string s)
        {
            if (s.Length == 0)
                return false;
            return s[0] == DirectorySeparator;
        }

        public static FileSystemPath Parse(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (!IsRooted(s))
                throw new Exception("path is not rooted");
            if (s.Contains(string.Concat(DirectorySeparator, DirectorySeparator)))
                throw new Exception("path contains double directory-separators");
            return new FileSystemPath(s);
        }

        public FileSystemPath AppendPath(string relativePath)
        {
            if (IsRooted(relativePath))
                throw new ArgumentException("the specified path should be relative", "relativePath");
            if (!IsDirectory)
                throw new InvalidOperationException("this path is not a directory");
            return new FileSystemPath(Path + relativePath);
        }

        public FileSystemPath AppendPath(FileSystemPath path)
        {
            if (!IsDirectory)
                throw new InvalidOperationException("this path is not a directory");
            return new FileSystemPath(Path + path.Path.Substring(1));
        }

        public FileSystemPath AppendDirectory(string directoryName)
        {
            if (directoryName.Contains(DirectorySeparator.ToString()))
                throw new ArgumentException("the specified name includes directory-separator(s)", "directoryName");
            if (!IsDirectory)
                throw new InvalidOperationException("the specified Path is not a directory");
            return new FileSystemPath(Path + directoryName + DirectorySeparator);
        }

        public FileSystemPath AppendFile(string fileName)
        {
            if (fileName.Contains(DirectorySeparator.ToString()))
                throw new ArgumentException("the specified name includes directory-separator(s)", "fileName");
            if (!IsDirectory)
                throw new InvalidOperationException("the specified Path is not a directory");
            return new FileSystemPath(Path + fileName);
        }

        public bool IsParentOf(FileSystemPath path)
        {
            return IsDirectory && Path.Length != path.Path.Length && path.Path.StartsWith(Path);
        }

        public bool IsChildOf(FileSystemPath path)
        {
            return path.IsParentOf(this);
        }

        public FileSystemPath RemoveParent(FileSystemPath parent)
        {
            if (!parent.IsDirectory)
                throw new ArgumentException("the specified path can not be the parent of this path: it is not a directory");
            if (!Path.StartsWith(parent.Path))
                throw new ArgumentException("the specified path is not a parent of this path");
            return new FileSystemPath(Path.Remove(0, parent.Path.Length - 1));
        }

        public FileSystemPath RemoveChild(FileSystemPath child)
        {
            if (!Path.EndsWith(child.Path))
                throw new ArgumentException("the specified path is not a child of this path");
            return new FileSystemPath(Path.Substring(0, Path.Length - child.Path.Length + 1));
        }

        public string GetExtension()
        {
            if (!IsFile)
                throw new ArgumentException("the specified Path is not a file");
            string name = EntityName;
            int extensionIndex = name.LastIndexOf('.');
            if (extensionIndex < 0)
                return "";
            return name.Substring(extensionIndex);
        }

        public FileSystemPath ChangeExtension(string extension)
        {
            if (!IsFile)
                throw new ArgumentException("the specified Path is not a file");
            string name = EntityName;
            int extensionIndex = name.LastIndexOf('.');
            if (extensionIndex >= 0)
                return ParentPath.AppendFile(name.Substring(0, extensionIndex) + extension);
            return Parse(Path + extension);
        }

        public string[] GetDirectorySegments()
        {
            FileSystemPath path = this;
            if (IsFile)
                path = path.ParentPath;
            var segments = new LinkedList<string>();
            while (!path.IsRoot)
            {
                segments.AddFirst(path.EntityName);
                path = path.ParentPath;
            }
            return segments.ToArray();
        }

        public int CompareTo(FileSystemPath other)
        {
            return Path.CompareTo(other.Path);
        }

        public override string ToString()
        {
            return Path;
        }

        public override bool Equals(object obj)
        {
            if (obj is FileSystemPath)
                return Equals((FileSystemPath)obj);
            return false;
        }

        public bool Equals(FileSystemPath other)
        {
            return other.Path.Equals(Path);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public static bool operator ==(FileSystemPath one, FileSystemPath other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(FileSystemPath one, FileSystemPath other)
        {
            return !(one == other);
        }
    }
}
