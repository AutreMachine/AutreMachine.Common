﻿using AutreMachine.Common.NetCDF;
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
            Func<float, double, float> multiply = (x, y) => (float)(x * y);

            var test = new ReadNC<MeanTemp, float>(path2, multiply);
            test.DisplayConsole();
            test.Process("lon", "lat", "time", "tasAdjust");
            var res = test.GetInterpolation(2.27, 48.81, 1020);
            Console.WriteLine("\n\nResult : ");
            Console.WriteLine(res.Content);
            Console.WriteLine((test.Results.lat.Length - 1) * (test.Results.lon.Length - 1) + " polygons.");

            Assert.That(test.Results, Is.Not.Null);
            Assert.That(test.Results.lat.Length > 0, Is.True);
            // Lat : 48.81, Lon : 2.25
        }
    }
}
