using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome.Lib
{
    public interface IGuidSupply
    {
        IGuidSupply NewID(out Guid id);
    }
}
