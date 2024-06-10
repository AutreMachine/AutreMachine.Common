using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public class DateTools
    {
        /// <summary>
        /// Get the start date of week for now
        /// </summary>
        /// <returns></returns>
        public static DateTime StartWeek()
        {
            var t = DateTime.UtcNow;
            // Get day of week
            int diff = (7 + (t.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startWeek = t.AddDays(-1 * diff).Date;

            return startWeek;
        }

        /// <summary>
        /// Get the number of day (0...6) from a date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int NumDay(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return diff;
        }

        public static DateTime TimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            var maxNbSeconds = DateTime.MaxValue.Subtract(dtDateTime).TotalSeconds;
            if (unixTimeStamp > maxNbSeconds)
                dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            else
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static long DateTimeToTimeStamp(DateTime date)
        {
            long epoch = (date.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        /// <summary>
        /// Creates a datetime from DateStart and NumDay
        /// </summary>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static DateTime? DateFromNumDay(DateTime firstDayOfWeek, int numDay, TimeSpan? time)
        {

            var d = firstDayOfWeek.AddDays(numDay);
            if (time.HasValue)
            {
                var date = new DateTime(d.Year, d.Month, d.Day, time.Value.Hours, time.Value.Minutes, time.Value.Seconds, DateTimeKind.Utc);
                return date;
            } else
            {
                var date = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc);
                return date;

            }
        }

        public static TimeSpan HoursToTimeSpan(double hours)
        {
            var hoursInt = (int)hours;
            var minutes = (int)((hours - (double)hoursInt) * 60.0);
            return new TimeSpan(hoursInt, minutes, 0);
        }
    }
}
