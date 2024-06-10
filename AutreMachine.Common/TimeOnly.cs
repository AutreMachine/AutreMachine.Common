using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public struct TimeOnlyExtensions
    {
        public static TimeOnly Max(TimeOnly t1, TimeOnly t2)
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
