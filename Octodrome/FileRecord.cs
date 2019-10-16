using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable enable

namespace Octodrome
{
    public class FileRecord
    {
        public FileRecordID ID { get; internal set; }
        public DateTime TimeStamp { get; internal set; }
        public string FileName => ID.ToString(TimeStamp);
        public string DirectoryPath(string root) =>
            Path.Combine(
                root,
                ID.Guid.ToString().Substring(0, 2)
            );
        public string FileNamePattern =>
            ID + "*";



    }
}
