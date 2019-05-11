using System;
using Newtonsoft.Json;

namespace Timecard.Models
{
    public class TimeWorked : IComparable<TimeWorked>
    {
        public static readonly string[] Hours;
        public static readonly string[] Minutes;

        static TimeWorked()
        {
            int maxHours = ProjectSettings.MaxNumberHoursInWorkDay;
            Hours = new string[maxHours + 2];

            Hours[0] = "Hours";
            for (int i = 0; i <= maxHours; i++)
                Hours[i + 1] = i.ToString();

            Minutes = new string[] { "Minutes", "00", "15", "30", "45" };
        }

        public string HoursPart { get; set; }
        public string MinutesPart { get; set; }

        public TimeWorked()
        {
            HoursPart = Hours[2];
            MinutesPart = Minutes[1];
        }

        public TimeWorked(string hours, string minutes)
        {
            HoursPart = hours;
            MinutesPart = minutes;
        }

        public string ToDecimalFormat()
        {
            var hours = float.Parse(HoursPart);
            var minutes = float.Parse(MinutesPart) / 60;

            return string.Format("{0:F2}", hours + minutes);
        }

        public string ToColonFormat()
        {
            return string.Format("{0}:{1}", HoursPart, MinutesPart);
        }

        public float AsFloat()
        {
            return float.Parse(ToDecimalFormat());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TimeWorked other))
            {
                return false;
            }

            return HoursPart == other.HoursPart && MinutesPart == other.MinutesPart;
        }

        public override int GetHashCode()
        {
            int result = 17;

            result = 31 * result + HoursPart.GetHashCode();
            result = 31 * result + MinutesPart.GetHashCode();

            return result;
        }

        public static TimeWorked FromDecimalFormat(string df)
        {
            var times = df.Split(".");
            string hours = times[0];

            string minutes;

            if (times.Length > 1)
            {
                switch (times[1])
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
            }
            else
            {
                minutes = "00";
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

        public int CompareTo(TimeWorked other)
        {
            return AsFloat().CompareTo(other.AsFloat());
        }
    }


    class TimeWorkedConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var timeWorked = value as TimeWorked;
            writer.WriteValue(timeWorked.AsFloat());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            float timeWorked = serializer.Deserialize<float>(reader);
            return TimeWorked.FromDecimalFormat(timeWorked.ToString("0.00"));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeWorked);
        }
    }
}
