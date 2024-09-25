using AutreMachine.Common.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.Tests.AI
{
    public class TestAI
    {
        IAIService _businessService;

         [SetUp]
        public void Setup()
        {

            _businessService = Helper.GetRequiredService<IAIService>();

        }

        [Test]
        public async Task Test_AI()
        {
            return;
            var questions = new string[] { "What is the amount of the \"Power Unit Cost Cap\" in USD, GBP and EUR", "What is the value of External Manufacturing Costs in USD", "What is the Capital Expenditure Limit in USD" };
            var ask = await _businessService.AskQuestionsOnPDF(questions, "AI\\fia_f1_power_unit_financial_regulations_issue_1_-_2022-08-16.pdf");

            Assert.That(ask.Succeeded, Is.True);
            foreach(var ans in ask.Content)
                Console.WriteLine(ans);


        }
    }
}
