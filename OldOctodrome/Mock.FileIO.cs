using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#nullable enable

namespace Octodrome.Mock
{
    /// <summary>
    /// For testing.
    /// </summary>
    public class FileIO: IFileIO
    {
        internal static ApplicationException Error(string msg) =>
            new ApplicationException(msg);
        internal static ApplicationException Nyi(string what) =>
            Error(what + " not yet implemented");
        internal readonly Mock.TimeStampSource TimeStampSource = new Mock.TimeStampSource();

        internal FileIO() { }

        public class _FileInfo: IFileInfo
        {
            public bool Exists { get; set; }
            public string Path { get; set; } = "";
            public string FileName { get; set; } = "";
            public string DirPath { get; set; } = "";
            public long Length { get; set; }
            public DateTime CreationTimeUtc { get; set; }
            public DateTime LastWriteTimeUtc { get; set; }
            internal string Content { get; set; } = "";
        }

        internal class Dir
        {
            internal string Path;
            internal Dictionary<string, Dir> Subdirs = new Dictionary<string, Dir> { };
            internal Dictionary<string, _FileInfo> Files = new Dictionary<string, _FileInfo> { };
            internal Dir(string path)
            {
                Path = path;
            }
        }

        internal class FileObj
        {
            internal string Path;
            internal DateTime CreationTimeUtc;
            internal DateTime LastWriteTimeUtc;
            internal string Data;
            internal FileObj(string path, string data, DateTime ts)
            {
                Path = path;
                Data = data;
                CreationTimeUtc = ts;
                LastWriteTimeUtc = ts;
            }
        }

        internal readonly Dir Root = new Dir("");

        internal Dir FindDir(string path)
        {
            var parent = Path.GetDirectoryName(path);
            if (parent == null || parent == @"\" || parent == "/") return Root;
            var last = Path.GetFileName(path);
            var parentDir = FindDir(parent);
            return (last == "" || last == null ? parentDir : parentDir.Subdirs[last]);
        }

        internal _FileInfo FindFileInfo(string path)
        {
            var dirPath = Path.GetDirectoryName(path) ?? "";
            var fileName = Path.GetFileName(path) ?? "";
            var dir = FindDir(dirPath);
            var fileInfo = dir.Files[fileName];
            return fileInfo;
        }

        public async Task<IEnumerable<string>> GetDirPathsAsync(string path, string pattern = "*", bool recursive = false) =>
            await Task.Run(() =>
            {
                var dir = FindDir(path);
                var re = new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$");
                var paths =
                    dir
                    .Subdirs
                    .Keys
                    .Where(d => re.IsMatch(d))
                    .Select(d => Path.Combine(dir.Path, d));
                if (recursive)
                {
                    paths =
                        paths
                        .SelectMany(p => GetDirPathsAsync(p, pattern, recursive).Result);
                }
                paths = paths.ToArray();
                return paths as IEnumerable<string>;
            });

        public async Task<IEnumerable<string>> GetFilePathsAsync(string path, string pattern = "*") =>
            await Task.Run(() =>
            {
                var dir = FindDir(path);
                var re = new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$");
                var paths =
                    dir
                    .Files
                    .Keys
                    .Where(d => re.IsMatch(d))
                    .Select(d => Path.Combine(dir.Path, d))
                    .ToArray();
                return paths as IEnumerable<string>;
            });

        public async Task<IFileInfo> GetFileInfoAsync(string path) =>
            await Task.Run(() =>
            {
                var fileInfo = FindFileInfo(path);
                return fileInfo;
            });

        public async Task<string> ReadAllTextAsync(string path) =>
            await Task.Run(() => FindFileInfo(path).Content);

        public Task<byte[]> ReadAllBytesAsync(string path) =>
            throw Nyi("ReadAllBytesAsync");

        public async Task WriteAllTextAsync(string path, string text) =>
            await Task.Run(() =>
            {
                var dirPath = Path.GetDirectoryName(path) ?? "";
                var fileName = Path.GetFileName(path) ?? "";
                var dir = FindDir(dirPath);
                lock (dir)
                {
                    if (dir.Files.ContainsKey(fileName))
                    {
                        var fileInfo = dir.Files[fileName];
                        lock (fileInfo) {
                            fileInfo.Content = text;
                            fileInfo.LastWriteTimeUtc = TimeStampSource.GetTimeStamp();
                        }
                    }
                    else
                    {
                        var timeStamp = TimeStampSource.GetTimeStamp();
                        var fileInfo = new _FileInfo
                        {
                            Path = path,
                            CreationTimeUtc = timeStamp,
                            LastWriteTimeUtc = timeStamp,
                            Content = text
                        };
                        dir.Files[fileName] = fileInfo;
                    }
                }
            });

        public Task WriteAllBytesAsync(string path, byte[] bytes) =>
            throw Nyi("WriteAllBytesAsync");

        public async Task AppendLinesAsync(string path, IEnumerable<string> lines) =>
            await Task.Run(() =>
            {
                var fileInfo = FindFileInfo(path);
                if (!fileInfo.Exists) throw Error($"File not found: {path}");
                lock (fileInfo)
                {
                    var timeStamp = TimeStampSource.GetTimeStamp();
                    foreach (var line in lines) fileInfo.Content += line;
                    fileInfo.LastWriteTimeUtc = timeStamp;
                }
            });

        public async Task AppendLineAsync(string path, string line) =>
            await AppendLinesAsync(path, new[] { line });

        public async Task DeleteFileAsync(string path) =>
            await Task.Run(() =>
            {
                var dirPath = Path.GetDirectoryName(path) ?? "";
                var fileName = Path.GetFileName(path) ?? "";
                var dir = FindDir(dirPath);
                dir.Files.Remove(fileName);
            });

        public async Task CreateDirAsync(string path) =>
            await Task.Run(async () =>
            {
                try
                {
                    FindDir(path);
                }
                catch
                {
                    var parentPath = Path.GetDirectoryName(path) ?? "";
                    var dirName = Path.GetFileName(path) ?? "";
                    await CreateDirAsync(parentPath);
                    var parentDir = FindDir(parentPath);
                    parentDir.Subdirs[dirName] = new Dir(path);
                }
            });

        public async Task DeleteDirAsync(string path, bool recursive = false) =>
            await Task.Run(() =>
            {
                var parentPath = Path.GetDirectoryName(path) ?? "";
                var dirName = Path.GetFileName(path) ?? "";
                try
                {
                    var parentDir = FindDir(parentPath);
                    if (!parentDir.Subdirs.ContainsKey(dirName))
                    {
                        throw Error($"No such directory: {path}");
                    }
                    if (!recursive)
                    {
                        var dir = parentDir.Subdirs[dirName];
                        if (dir.Subdirs.Any() || dir.Files.Any())
                        {
                            throw Error($"Directory is not empty: {path}");
                        }
                    }
                    parentDir.Subdirs.Remove(dirName);
                }
                catch
                {
                    throw Error($"No such directory: {path}");
                }
            });

        internal string CurrTime => TimeStampSource.CurrTime.ToString("HH:mm:ss");
        public async Task LogInfoAsync(string msg) =>
            await Console.Out.WriteLineAsync($"{CurrTime} INFO: {msg}");
        public async Task LogWarnAsync(string msg) =>
            await Console.Out.WriteLineAsync($"{CurrTime} WARN: {msg}");
        public async Task LogErrorAsync(string msg) =>
            await Console.Out.WriteLineAsync($"{CurrTime} ERROR: {msg}");
        public async Task LogExceptionAsync(string msg, Exception e) =>
            await Console.Out.WriteLineAsync($"{CurrTime} EXCEPTION: {msg}: {string.Join("; ", Util.ExceptionMsgs(e))}");
    }
}
