using System;
namespace Timecard.Models
{
    public static class ProjectSettings
    {
        public static readonly string DateFormat = "dddd, MMMM dd, yyyy";
        public static readonly DayOfWeek PayPeriodStartDay = DayOfWeek.Wednesday;
        public static readonly DayOfWeek PayPeriodEndDay = DayOfWeek.Tuesday;
        public static readonly uint NumberWeeksInPayPeriod = 1;

        public static readonly string[] OtherTimeOptions = {
                "Shop", "Vacation", "Holiday", "Sick"
            };
    }
}
