using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace Octodrome
{
    public class FileRecordPrefix
    {
        internal readonly string Pfx;
        public override string ToString() => Pfx;
        public static FileRecordPrefix Of(string pfx)
        {
            if (pfx.Any(x => !char.IsLetterOrDigit(x)))
            {
                pfx = string.Concat(pfx.Where(char.IsLetterOrDigit));
            }
            return new FileRecordPrefix(pfx);
        }
        internal FileRecordPrefix(string str)
        {
            Pfx = str;
        }
    }
}
