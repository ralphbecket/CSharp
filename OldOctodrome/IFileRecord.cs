using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Octodrome
{
    /// <summary>
    /// A file record 
    /// </summary>
    public interface IFileRecord
    {
        FileRecordID ID { get; }
        DateTime TimeStamp { get; }
        string FileName { get; }
        string DirectoryPath(string root);
        string FileNamePattern { get; }
        IDictionary<string, string> Fields { get; }
    }
}
