using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome
{
    public interface IFileInfo
    {
        bool Exists { get; }
        string Path { get; }
        string FileName { get; }
        string DirPath { get; }
        long Length { get; }
        DateTime CreationTimeUtc { get; }
        DateTime LastWriteTimeUtc { get; }
    }
}
