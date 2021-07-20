using System;
using System.Collections.Generic;

namespace Octodrome.Lib
{
    /// <summary>
    /// An item in the document graph.
    /// </summary>
    public interface IItem
    {
        public Guid ID { get; }
        public Doc DeepClone(Doc doc, out IItem clone, Dictionary<Guid, IItem>? clones = null);
    }
}
