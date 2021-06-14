using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octodrome.DB
{
    public interface ITable
    {
        string Label { get; }
        IEnumerable<IColumn> Columns { get; }
        bool IsDeletable { get; }
        void Delete();
        void Undelete();
        bool IsToBeDeleted { get; }
    }
}
