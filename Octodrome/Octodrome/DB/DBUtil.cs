using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octodrome.DB
{
    public static class DBUtil
    {
        public static int ParseInt(string s, int min = int.MinValue, int max = int.MaxValue)
        {
            var x = 0;
            try
            {
                x = int.Parse(s.Trim());
            }
            catch
            {
                throw new ApplicationException("This must be a whole number.");
            }
            if (int.MinValue != min && x < min)
            {
                throw new ApplicationException($"This must be at least {min}.");
            }
            if (int.MaxValue != max && max < x)
            {
                throw new ApplicationException($"This must be at most {max}.");
            }
            return x;
        }
        public static double ParseDouble(string s, double min = double.MinValue, double max = double.MaxValue)
        {
            var x = 0.0;
            try
            {
                x = double.Parse(s.Trim());
            }
            catch
            {
                throw new ApplicationException("This must be a whole number.");
            }
            if (double.MinValue != min && x < min)
            {
                throw new ApplicationException($"This must be at least {min}.");
            }
            if (double.MaxValue != max && max < x)
            {
                throw new ApplicationException($"This must be at most {max}.");
            }
            return x;
        }
        public static string TrimmedText(string s, bool allowEmpty = true)
        {
            s = s.Trim();
        if (!allowEmpty && s == "")
            {
                throw new ApplicationException("This is required.");
            }
            return s;
        }
    }
}
