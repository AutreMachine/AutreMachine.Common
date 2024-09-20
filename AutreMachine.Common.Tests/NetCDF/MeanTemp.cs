using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Tests.NetCDF
{
    public class MeanTemp
    {
        public double[] lat { get; set; }
        public double[] lon { get; set; }
        public double height { get; set; }
        public long[] time { get; set; }
        public float[,,] tasAdjust { get; set; }
    }
}
