using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Octodrome
{
    /// <summary>
    /// This abstracts file IO so we can implement a fake IO system for testing.
    /// File reads and writes must be atomic.
    /// </summary>
    public interface IFileIO
    {
        Task<IEnumerable<string>> GetDirPathsAsync(string path, string pattern = "*", bool recursive = false);
        Task<IEnumerable<string>> GetFilePathsAsync(string path, string pattern = "*");
        Task<IFileInfo> GetFileInfoAsync(string path);
        Task<string> ReadAllTextAsync(string path);
        Task<byte[]> ReadAllBytesAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        Task WriteAllBytesAsync(string path, byte[] bytes);
        Task AppendLinesAsync(string path, IEnumerable<string> lines);
        Task AppendLineAsync(string path, string line);
        Task DeleteFileAsync(string path);
        Task CreateDirAsync(string path);
        Task DeleteDirAsync(string path, bool recursive = false);
        Task LogInfoAsync(string msg);
        Task LogWarnAsync(string msg);
        Task LogErrorAsync(string msg);
        Task LogExceptionAsync(string msg, Exception e);
    }
}
