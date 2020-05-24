using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Octodrome
{
    /// <summary>
    /// Each file record has an ID consisting of a prefix, which is a
    /// string designed to help the developer distinguish the nature
    /// of the record, and a GUID, which uniquely distinguishes the
    /// record.
    /// 
    /// File records may be updated, but the older versions are never
    /// deleted.  Each version of a file record is uniquely identified
    /// by the time stamp of its creation.
    /// </summary>
    public class FileRecordID
    {
        internal readonly FileRecordPrefix Pfx;
        internal readonly Guid Guid;
        public override string ToString() => Pfx + "." + Guid;
        public string ToString(DateTime timeStamp) =>
            Pfx + "." +
            Guid + "." +
            TimeZoneInfo.ConvertTimeToUtc(timeStamp).ToString("O");
        public static FileRecordID Of(FileRecordPrefix pfx, Guid guid) => new FileRecordID(pfx, guid);
        internal FileRecordID(FileRecordPrefix pfx, Guid guid)
        {
            Pfx = pfx;
            Guid = guid;
        }
    }
}
