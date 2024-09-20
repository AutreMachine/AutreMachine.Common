using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{

    public static class ReducedString
    {
        /// <summary>
        /// Reduces string to the MaxLength provided
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns></returns>
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
