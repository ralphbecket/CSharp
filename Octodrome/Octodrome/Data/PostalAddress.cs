using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octodrome.DB;

namespace Octodrome.Data
{
    public class PostalAddress: ContactDetails
    {
        public int ID;
        public PostalAddress() :
            base("Postal address",
                new Column<string>("Addressee", "", x => DBUtil.TrimmedText(x), placeholder: "(if different)"),
                new Column<string>("Line 1", "", x => DBUtil.TrimmedText(x, allowEmpty: false)),
                new Column<string>("Line 2", "", x => DBUtil.TrimmedText(x)),
                new Column<string>("State", "", x => DBUtil.TrimmedText(x, allowEmpty: false)),
                new Column<string>("Post code", "", x => DBUtil.TrimmedText(x)),
                new Column<string>("Country", "", x => DBUtil.TrimmedText(x), placeholder: "(if abroad)")
            )
        { }
    }
}
