using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome
{
    public interface ITimeStampSource
    {
        DateTime GetTimeStamp();
    }
}
