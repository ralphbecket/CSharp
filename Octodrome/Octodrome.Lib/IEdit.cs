using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Octodrome.Lib
{
    public interface IEdit
    {
        public Guid ItemID { get; }
        public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items);
        public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items);
    }
}
