﻿using System;
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
                dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
            else
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        public static long AddTimeSpanToTimeStamp(long timestamp, TimeSpan timeSpan)
        {
            var date = TimeStampToDateTime(timestamp);
            date = date.Add(timeSpan);
            return DateTimeToTimeStamp(date);
        }

        public static long DateTimeToTimeStamp(DateTime date)
        {
            long epoch = (date.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        public static bool IsUTC(DateTime date)
        {
            
            var compareUTC = new DateTime(date.Ticks, DateTimeKind.Utc);
            return date.ToLocalTime() == compareUTC.ToLocalTime();
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

        /// <summary>
        /// Displays date under : 2days... or 2 hours...
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string VeryShortDate(DateTime date, bool isShort = false, DateTime? refDate = null)
        {
            var res = "";
            TimeSpan delta;
            bool isPast = false;
            // Get the delta
            DateTime now = DateTime.UtcNow;
            if (refDate != null)
                now = refDate.Value;
            else
            {
                if (!IsUTC(date))
                    now = DateTime.Now;
            }

            if (now < date)
            {
                // in the future
                delta = date.Subtract(now);
            }
            else
            {
                delta = now.Subtract(date);
                isPast = true;
            }
            var years = isShort ? "y" : "years";
            var year = isShort ? "y" : "year";
            var months = isShort ? "m" : "months";
            var month = isShort ? "m" : "month";
            var days = isShort ? "d" : "days";
            var day = isShort ? "d" : "day";
            var hours = isShort ? "h" : "hrs";
            var hour = isShort ? "h" : "hr";
            var minutes = isShort ? "m" : "mins";
            var minute = isShort ? "m" : "min";

            // time elapsed
            var monthsElapsed = 0; // (int)(delta.TotalDays/30.0);
            if (isPast)
                for (int i = 0; i < delta.TotalDays; i++)
                {
                    var current = date.AddDays(i);
                    if (current.Day == now.Day && current.Month < now.Month)
                        monthsElapsed++;
                }
            else
                for (int i = 0; i < delta.TotalDays; i++)
                {
                    var current = now.AddDays(i);
                    if (current.Day == date.Day && current.Month < date.Month)
                        monthsElapsed++;
                }
            var yearsElapsed = (int)(delta.TotalDays / 365.0);

            if (delta.Days > 9999)
                res = $"<i class='fa-solid fa-infinity'></i> {days}";
            else if (yearsElapsed > 1) 
                res = $"{yearsElapsed} {years}";
            else if (yearsElapsed == 1)
                res = $"{yearsElapsed} {year}";
            else if (monthsElapsed > 1) 
                res = $"{monthsElapsed} {months}";
            else if (monthsElapsed == 1)
                res = $"1 {month}";
            else if (delta.Days > 1)
                res = $"{delta.Days} {days}";
            else if (delta.Days == 1)
                res = $"1 {day}";
            else if (delta.Hours > 1)
                res = $"{delta.Hours} {hours}";
            else if (delta.Hours == 1)
                res = $"1 {hour}";
            else if (delta.Minutes > 1)
                res = $"{delta.Minutes} {minute}";
            else if (delta.Minutes == 1)
                res = "1 min";
            else
                res = $"{delta.Seconds} s";

            if (isPast && !isShort)
                res = res + " ago";

            return res;

        }

    }
}
