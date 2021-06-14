using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octodrome.DB;

namespace Octodrome.Data
{
    public class PersonName: Table
    {
        public int ID;
        public int PartyID;
        public PersonName() : base(
            "Name",
            new Column<string>("Salutation", "", x => DBUtil.TrimmedText(x), placeholder: "Mr, Mrs, etc."),
            new Column<string>("First name", "", x => DBUtil.TrimmedText(x)),
            new Column<string>("Middle names", "", x => DBUtil.TrimmedText(x)),
            new Column<string>("Surname", "", x => DBUtil.TrimmedText(x)),
            new Column<string>("Preferred name", "", x => DBUtil.TrimmedText(x), placeholder: "(if any)")
        ) { }
    }
}
