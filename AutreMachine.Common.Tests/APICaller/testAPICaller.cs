using AutreMachine.Common.AI;
using AutreMachine.Common.Tests.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Tests.APICaller
{
    public class TestAPICaller
    {
         [SetUp]
        public void Setup()
        {


        }

        [Test]
        public async Task Test_GET()
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri( "https://localhost:7217"); // Does not exist
            try
            {
                var test = await APICaller<string>.Get(http, "api/dont_exist", 123);
                if (!test.Succeeded)
                    Console.WriteLine("Error : " + test.Message);
            }
            catch (Exception ex)
            {
                            Console.WriteLine("Error : " + ex.Message);

            }


        }
    }
}
