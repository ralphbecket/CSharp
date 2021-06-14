using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octodrome.DB;

namespace Octodrome.Data
{
    public abstract class ContactDetails: Table
    {
        public ContactDetails(string label, params IColumn[] columns)
            : base(label, columns) { }
    }
}
