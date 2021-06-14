using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome.Lib
{
    public static class Ex
    {
        public static ApplicationException Internal(string msg) =>
            new ApplicationException(msg);
        public static ApplicationException ObserverThrewAnException(Exception inner) =>
            new ApplicationException("item observer threw an exception", inner);
        public static ApplicationException ItemIDIsNotSet =>
            new ApplicationException("item ID is not set");
        public static ApplicationException ItemTypesDoNotMatch =>
            new ApplicationException("item types do not match");
        public static ApplicationException ItemIDsDoNotMatch =>
            new ApplicationException("item IDs do not match");
    }
}
