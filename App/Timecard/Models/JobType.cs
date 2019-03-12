using System;
using Newtonsoft.Json;

namespace Timecard.Models
{
    public enum JobType
    {
        Construction,
        Service,
        Other
    }

    class JobTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jobType = (JobType)value;
            writer.WriteValue(jobType.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobTypeString = existingValue as string;
            return (JobType)Enum.Parse(typeof(JobType), jobTypeString);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JobType);
        }
    }
}
