using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Octodrome.Mock
{
    public class TimeStampSource: ITimeStampSource
    {
        internal DateTime CurrTime = new DateTime(2019, 01, 01);
        public DateTime GetTimeStamp()
        {
            CurrTime = CurrTime.AddSeconds(1);
            return CurrTime;
        }
    }
}
