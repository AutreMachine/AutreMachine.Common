using AutreMachine.Common.NetCDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Tests.NetCDF
{
    public class TestNetCDF
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_ReadNC()
        {
            // Local test
            var path2 = "C:\\Users\\Utilisateur\\Downloads\\998b2a796d0a4c4eb8fedf8b68bd8565\\01_mean_temperature-projections-monthly-rcp_8_5-wrf381p-ipsl_cm5a_mr-r1i1p1-grid-v1.0.nc";

            // Read file and create object in return
            //var test = new TestNetCDF.ReadNC<MeanTemp>();
            //var result = test.Process(path2);
            var test = new ReadNC<MeanTemp>(path2);
            var result = test.Process();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.lat.Length > 0, Is.True);
        }
    }
}
