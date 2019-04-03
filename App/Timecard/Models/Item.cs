using System;
using Newtonsoft.Json;
using Timecard.Models;

namespace Timecard
{
    public class Item
    {
        // We do want to deserialize the Id but don't want to serialize it.
        // Internal get prevents serialization
        public string Id { internal get; set; }

        public CostCode CostCode { get; set; }

        [JsonConverter(typeof(JobDateConverter))]
        public DateTime JobDate { get; set; }

        [JsonConverter(typeof(JobTypeConverter))]
        public JobType JobType { get; set; }

        [JsonConverter(typeof(JobConverter))]
        public Job Job { get; set; }

        [JsonConverter(typeof(TimeWorkedConverter))]
        public TimeWorked TimeWorked { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    class JobDateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int seconds = DateTime.Now.Second;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType == JsonToken.Integer)
                {
                    int tempSec = Convert.ToInt32(reader.Value);
                    if (tempSec > 0)
                    {
                        seconds = tempSec;
                    }
                }
            }

            var jobDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            jobDate = jobDate.AddSeconds(seconds);

            return jobDate;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = (DateTime)value;
            writer.WriteValue(date.ToString());
        }
    }
}
