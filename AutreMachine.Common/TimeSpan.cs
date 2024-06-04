using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Concrete.CommonTools
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan AddHours(this TimeSpan t1, int h)
        {
            return t1.Add(new TimeSpan(h, 0, 0));
        }

        public static TimeSpan Max(TimeSpan t1, TimeSpan t2)
        {
            if (t1.Ticks > t2.Ticks)
                return t1;
            else
                return t2;

        }

        public static TimeOnly Min(TimeOnly t1, TimeOnly t2)
        {
            if (t1.Ticks < t2.Ticks)
                return t1;
            else
                return t2;

        }
    }
}
