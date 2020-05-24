using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome
{
    public class MockGuidSource: IGuidSource
    {
        internal int N = 0;
        public Guid GetNewGuid() => Guid.Parse($"00000000-0000-0000-0000-{N++,000000000000}");
    }
}
