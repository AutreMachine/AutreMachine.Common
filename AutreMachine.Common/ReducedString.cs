using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public static class ReducedString
    {
        public static string ToReducedString(this string str, int maxLength = 10)
        {
            if (str == null)
                return str;

            if (str.Length <= maxLength) 
                return str;

            return str.Substring(0, maxLength) + "...";
        }
    }
}
