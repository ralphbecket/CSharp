using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Octodrome.DB;

namespace Octodrome.Data
{
    public class WebAddress: ContactDetails
    {
        public int ID;
        public WebAddress() :
            base("Web address",
                new Column<string>("Description", "", x => DBUtil.TrimmedText(x)),
                new Column<string>("URL", "", x => DBUtil.TrimmedText(x, allowEmpty: false))
            )
        { }
    }
}
