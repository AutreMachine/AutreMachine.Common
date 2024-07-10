using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Razor
{
    public abstract class BaseClass : IBaseClass //, ICloneable
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        
    }
}
