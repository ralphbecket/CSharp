using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octodrome
{
    internal static class Util
    {
        internal static IEnumerable<string> ExceptionMsgs(Exception e)
        {
            yield return e.Message;
            if (e.InnerException == null) yield break;
            if (e is AggregateException ae)
            {
                foreach (var emsg in ae.InnerExceptions.SelectMany(eee => ExceptionMsgs(eee)))
                {
                    yield return emsg;
                }
            }
            else
            {
                foreach (var emsg in ExceptionMsgs(e.InnerException))
                {
                    yield return emsg;
                }
            }
        }
    }
}
