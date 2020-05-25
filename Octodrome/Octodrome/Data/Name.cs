using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octodrome.Data
{
    public class Name
    {
        public int ID;
        public int PartyID;
        /// <summary>
        /// This is a surname for an individual, otherwise an organisation name.
        /// </summary>
        public string PrimaryName;
        /// <summary>
        /// This only applies to individuals.
        /// </summary>
        public string FirstName;
        /// <summary>
        /// This only applies to individuals.
        /// </summary>
        public string MiddleNames;
        /// <summary>
        /// This only applies to individuals.
        /// </summary>
        public string PreferredName;
        /// <summary>
        /// This only applies to individuals.
        /// </summary>
        public string Salutation;
    }
}
