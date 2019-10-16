using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Octodrome
{
    public partial class FileIO: IFileIO
    {
        internal readonly TextWriter ConsoleLogInfo;
        internal readonly TextWriter ConsoleLogWarn;
        internal readonly TextWriter ConsoleLogError;
        internal readonly string? LogPath;
        internal readonly ITimeStampSource TimeStamps;

        public class _FileInfo: IFileInfo
        {
            internal FileInfo FI;
            internal _FileInfo(string path)
            {
                FI = new FileInfo(path);
            }
            public bool Exists => FI.Exists;
            public string Path => FI.FullName;
            public string FileName => FI.Name;
            public string DirPath => FI.DirectoryName;
            public long Length => FI.Length;
            public DateTime CreationTimeUtc => FI.CreationTimeUtc;
            public DateTime LastWriteTimeUtc => FI.LastWriteTimeUtc;
        }

        public FileIO(
            TextWriter? consoleLogInfo = null,
            TextWriter? consoleLogWarn = null,
            TextWriter? consoleLogError = null,
            string? logPath = null,
            ITimeStampSource? timeStampSource = null
        ) {
            ConsoleLogInfo = consoleLogInfo ?? Console.Out;
            ConsoleLogWarn = consoleLogWarn ?? consoleLogInfo ?? Console.Out;
            ConsoleLogError = consoleLogError ?? consoleLogWarn ?? consoleLogInfo ?? Console.Error;
            LogPath = logPath;
            TimeStamps = timeStampSource ?? new TimeStampSource();
        }

        public async Task<IEnumerable<string>> GetDirPathsAsync(string path, string pattern = "*", bool recursive = false) =>
            await Task.Run(() => Directory.EnumerateDirectories(
                path,
                pattern,
                recursive ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories
            ));
        public async Task<IEnumerable<string>> GetFilePathsAsync(string path, string pattern = "*") =>
            await Task.Run(() => Directory.EnumerateFiles(path, pattern));
        public async Task<IFileInfo> GetFileInfoAsync(string path) =>
            await Task.Run(() => new _FileInfo(path));
        public async Task<string> ReadAllTextAsync(string path) =>
            await File.ReadAllTextAsync(path);
        public async Task<byte[]> ReadAllBytesAsync(string path) =>
            await File.ReadAllBytesAsync(path);
        public async Task WriteAllTextAsync(string path, string text) =>
            await File.WriteAllTextAsync(path, text);
        public async Task WriteAllBytesAsync(string path, byte[] bytes) =>
            await File.WriteAllBytesAsync(path, bytes);
        public async Task AppendLineAsync(string path, string text) =>
            await File.AppendAllLinesAsync(path, Enumerable.Repeat(text, 1));
        public async Task AppendLinesAsync(string path, IEnumerable<string> lines) =>
            await File.AppendAllLinesAsync(path, lines);
        public async Task CreateDir(string path) =>
            await Task.Run(() => { Directory.CreateDirectory(path); });
        public async Task DeleteFileAsync(string path) =>
            await Task.Run(() => { File.Delete(path); });
        public async Task DeleteDirAsync(string path, bool recursive = false) =>
            await Task.Run(() => { Directory.Delete(path, recursive: recursive); });
        public async Task CreateDirAsync(string path) =>
            await Task.Run(() => { Directory.CreateDirectory(path); });
        internal async Task LogAsync(string msg, TextWriter console)
        {
            await console.WriteLineAsync(msg);
            if (LogPath == null) return;
            msg = TimeStamps.GetTimeStamp().ToString("yyyy-MM-dd hh:mm:ss ") + msg;
            await File.AppendAllTextAsync(LogPath, msg);
        }
        public async Task LogInfoAsync(string msg) =>
            await LogAsync("INFO " + msg, ConsoleLogInfo);
        public async Task LogWarnAsync(string msg) =>
            await LogAsync("WARN " + msg, ConsoleLogWarn);
        public async Task LogErrorAsync(string msg) =>
            await LogAsync("ERROR " + msg, ConsoleLogError);
        public async Task LogExceptionAsync(string msg, Exception e)
        {
            msg = $"ERROR {msg}: {string.Join("; ", Util.ExceptionMsgs(e))}";
            await LogAsync(msg, ConsoleLogError);
        }
    }
}
