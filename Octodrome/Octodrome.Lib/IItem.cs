using System;

namespace Octodrome.Lib
{
    /// <summary>
    /// An item in the document graph.
    /// </summary>
    public interface IItem
    {
        public Guid ID { get; }
    }
}
