using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Razor
{
    public abstract class BaseClass : IBaseIdClass //, ICloneable
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        
    }
}
