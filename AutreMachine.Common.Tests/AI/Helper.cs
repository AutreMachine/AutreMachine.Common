using AutreMachine.Common.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Tests.AI
{
    public static class Helper
    {
        private static IServiceProvider Provider()
        {
            //string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=OpenAIRSSDB;Trusted_Connection=True;";
            //string connectionString = "Server=tcp:concretesqlserver.database.windows.net,1433;Initial Catalog=openai-database;Persist Security Info=False;User ID=adminconcrete;Password=C6PQQse#TKF8;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var services = new ServiceCollection();



            services.AddHttpClient();

            services.AddScoped<IAIService, LocalLMStudioService>();

            return services.BuildServiceProvider();

        }

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            return provider.GetRequiredService<T>();
        }
    }
}
