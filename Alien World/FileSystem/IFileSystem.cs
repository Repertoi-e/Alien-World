﻿using System;
using System.IO;
using System.Collections.Generic;

namespace Alien_World.File_System
{
    public interface IFileSystem : IDisposable
    {
        ICollection<FileSystemPath> GetEntities(FileSystemPath path);
        bool Exists(FileSystemPath path);
        Stream CreateFile(FileSystemPath path);
        Stream OpenFile(FileSystemPath path, FileAccess access);
        void CreateDirectory(FileSystemPath path);
        void Delete(FileSystemPath path);
    }
}
