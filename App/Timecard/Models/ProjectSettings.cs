using System;
using System.Globalization;

namespace Timecard.Models
{
    public static class ProjectSettings
    {
        public static readonly string DateFormat = "dddd, MMMM dd, yyyy";
        public static readonly DayOfWeek PayPeriodStartDay = DayOfWeek.Wednesday;
        public static readonly DayOfWeek PayPeriodEndDay = DayOfWeek.Tuesday;
        public static readonly int NumberWeeksInPayPeriod = 1;

        public static readonly int NumberHoursInWorkDay = 8;
        public static readonly int NumberDaysInWorkWeek = 5;
        public static readonly int NumberHoursInWorkWeek = NumberHoursInWorkDay * NumberDaysInWorkWeek;
        public static readonly int MaxNumberHoursInWorkDay = 2 * NumberHoursInWorkDay - 1;

        public static DateTime GetStartOfCurrentPayPeriod()
        {
            var payPeriodStart = DateTime.Now;
            while (payPeriodStart.DayOfWeek != PayPeriodStartDay) 
            {
                payPeriodStart = payPeriodStart.AddDays(-1);
            }
            return payPeriodStart;
        }

        public static DateTime GetEndOfCurrentPayPeriod() 
        {
            var payPeriodEnd = DateTime.Now;
            while (payPeriodEnd.DayOfWeek != PayPeriodEndDay)
            {
                payPeriodEnd = payPeriodEnd.AddDays(1);
            }
            return payPeriodEnd;
        }

        public static DateTime? LocalDateFromString(string s)
        {
            var isValid = DateTime.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture, 
                                                 DateTimeStyles.AssumeLocal, out DateTime date);
            if (isValid)
                return date;
            return null;
        }
    }
}
