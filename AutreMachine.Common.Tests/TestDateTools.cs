using System.ComponentModel.DataAnnotations;

namespace AutreMachine.Common.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_DateTools()
        {
            var date = DateTime.Now;
            var dateUTC = DateTime.UtcNow;
            var timespan = DateTools.DateTimeToTimeStamp(date);
            var timespanUTC = DateTools.DateTimeToTimeStamp(dateUTC);
            var date2 = DateTools.TimeStampToDateTime(timespan);
            var dateUTC2 = DateTools.TimeStampToDateTime(timespanUTC);
            Console.WriteLine($"Comparing {date.Ticks} to {date2.Ticks}");
            Assert.That(date2.ToString() == date.ToString());
            Console.WriteLine($"Comparing {dateUTC.Ticks} to {dateUTC2.Ticks}");
            Assert.That(dateUTC.ToString() == dateUTC2.ToString());

            if (DateTools.IsUTC(date))
                Console.WriteLine("Date is UTC");
            if (DateTools.IsUTC(dateUTC))
                Console.WriteLine("DateUTC is UTC");

        }
    }
}