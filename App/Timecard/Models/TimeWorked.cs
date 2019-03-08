namespace Timecard.Models
{
    public class TimeWorked
    {
        public static readonly string[] Hours;
        public static readonly string[] Minutes;

        static TimeWorked()
        {
            int maxHours = ProjectSettings.MaxNumberHoursInWorkDay;
            Hours = new string[maxHours + 1];

            for (int i = 0; i < maxHours; i++)
                Hours[i] = i.ToString();

            Minutes = new string[]{ "00", "15", "30", "45" };
        }

        private readonly string hoursPart;
        private readonly string minutesPart;

        public TimeWorked(string hours, string minutes)
        {
            hoursPart = hours;
            minutesPart = minutes;
        }

        public string ToDecimalFormat()
        {
            var minutes = float.Parse(minutesPart) / 60;
            return string.Format($"{0}.{1}", hoursPart, minutes.ToString("0.00"));
        }

        public string ToColonFormat()
        {
            return string.Format($"{0}:{1}", hoursPart, minutesPart);
        }

        public static TimeWorked FromDecimalFormat(string df)
        {
            var times = df.Split(".");
            string hours = times[0];

            string minutes;
            switch(times[1])
            {
                case "75":
                    minutes = "45";
                    break;
                case "50":
                    minutes = "30";
                    break;
                case "25":
                    minutes = "15";
                    break;
                default:
                    minutes = "00";
                    break;
            }

            return new TimeWorked(hours, minutes);
        }

        public static TimeWorked FromColonFormat(string cf)
        {
            var times = cf.Split(":");
            string hours = times[0];
            string minutes = times[1];

            return new TimeWorked(hours, minutes);
        }
    }
}
