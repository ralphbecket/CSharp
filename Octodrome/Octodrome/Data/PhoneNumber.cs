using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octodrome.DB;

namespace Octodrome.Data
{
    public class PhoneNumber : ContactDetails
    {
        public int ID;
        public PhoneNumber() :
            base("Phone number",
                new Column<string>(
                    "Kind",
                    "",
                    parse: x => DBUtil.TrimmedText(x),
                    placeholder: "Work, home, mobile, etc."
                ),
                new Column<string>(
                    "Number",
                    "",
                    parse: x => {
                        x = DBUtil.TrimmedText(x, allowEmpty: false);
                        if (!x.All(c => "0123456789 -+()".Contains(c)))
                        {
                            throw new ApplicationException("This doesn't look like a phone number.");
                        }
                        return x;
                    }
                )
            )
        { }
    }
}
