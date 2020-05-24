using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome
{
    public interface IGuidSource
    {
        Guid GetNewGuid();
    }
}
