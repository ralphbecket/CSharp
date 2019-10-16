using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome
{
    public class TimeStampSource: ITimeStampSource
    {
        public DateTime GetTimeStamp() => DateTime.Now;
    }
}
