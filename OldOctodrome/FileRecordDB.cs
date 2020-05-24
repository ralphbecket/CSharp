using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Octodrome
{
    /// <summary>
    /// A file database supports the reading, writing, and searching
    /// of file records.  The file DB consists of two main parts: a
    /// file record repository which stores all file records; and a
    /// set of indices, which are used to search for file records.
    /// Each search strategy may require its own index.
    /// </summary>
    public class FileRecordDB
    {
        internal readonly IFileIO IO;
        internal readonly IGuidSource Guids;
        internal readonly ITimeStampSource TimeStamps;
        internal readonly string Root;

        public FileRecordDB(IFileIO io, IGuidSource guids, ITimeStampSource timeStamps, string root)
        {
            IO = io;
            Guids = guids;
            TimeStamps = timeStamps;
            Root = root;
        }

        public class WriteFileRecordResult
        {

        }

        public async Task<WriteFileRecordResult> WriteFileRecord(IFileRecord fr)
        {
            var dir = fr.DirectoryPath(Root);
            var pattern = fr.FileNamePattern;
            var paths = await IO.GetFilePathsAsync(dir, pattern);
            var currPath = paths.LastOrDefault();
            if (currPath == null)
            {
                return await CreateFileRecord(fr, dir);
            }
            else
            {
                return await UpdateFileRecord(fr, dir, currPath);
            }
        }

        internal async Task<WriteFileRecordResult> CreateFileRecord(IFileRecord fr, string dir)
        {
            return null; // XXX
        }

        internal async Task<WriteFileRecordResult> UpdateFileRecord(IFileRecord fr, string dir, string currPath)
        {
            return null; // XXX
        }
    }
}
