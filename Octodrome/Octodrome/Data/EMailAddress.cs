using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octodrome.DB;

namespace Octodrome.Data
{
    public class EMailAddress: ContactDetails
    {
        public int ID;
        private static readonly string EMailRegexPattern =
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        private static readonly Regex EMailRegex =
            new Regex(EMailRegexPattern);

        public EMailAddress() :
            base("E-mail",
                new Column<string>(
                    "E-mail address",
                    "",
                    x =>
                    {
                        x = x.Trim();
                        if (!EMailRegex.IsMatch(x))
                        {
                            throw new ApplicationException(
                                "This must be a valid e-mail address."
                            );
                        }
                        return x;
                    }
                )
            )
        {
        }
    }
}
