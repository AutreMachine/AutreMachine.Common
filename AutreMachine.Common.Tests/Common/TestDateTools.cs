using System.ComponentModel.DataAnnotations;
using AutreMachine.Common.AI;
using AutreMachine.Common.Tests.AI;

namespace AutreMachine.Common.Tests.Common
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

        [Test]
        public void Test_Timestamp()
        {
            var date = DateTime.Now;
            var timestamp1 = DateTools.DateTimeToTimeStamp(date);
            var timestamp2 = DateTools.AddTimeSpanToTimeStamp(timestamp1, new TimeSpan(1, 0, 0));
            var date2 = DateTools.TimeStampToDateTime(timestamp2);
            Console.WriteLine($"Comparing {date} to {date2}");

        }

        [Test]
        public void Test_VeryShortDate()
        {
            // Years
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var since = date.AddYears(-3);
            var str = DateTools.VeryShortDate(since);

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("3 years ago"));

            since = date.AddYears(-1);
            str = DateTools.VeryShortDate(since);

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("1 year ago"));

            // Months
            since = date.AddMonths(-6);
            str = DateTools.VeryShortDate(since);

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("6 months ago"));

            since = date.AddDays(-65);
            str = DateTools.VeryShortDate(since);

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("2 months ago"));

            since = date.AddMonths(-1);
            str = DateTools.VeryShortDate(since);

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("1 month ago"));

            // Days
            since = date.AddDays(-1);
            str = DateTools.VeryShortDate(since); //, false, date);
            var elapsed = date.Subtract(since).Days;

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("1 day ago"));

            since = date.AddDays(1);
            str = DateTools.VeryShortDate(since);

            Console.WriteLine($"{since} : {str}");
            Assert.That(str, Is.EqualTo("1 day"));

        }
    }
}