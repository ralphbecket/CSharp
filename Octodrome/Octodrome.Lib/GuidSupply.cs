using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome.Lib
{
    public struct GuidSupply: IGuidSupply
    {
        public IGuidSupply NewID(out Guid id)
        {
            id = Guid.NewGuid();
            return this;
        }
    }
}
