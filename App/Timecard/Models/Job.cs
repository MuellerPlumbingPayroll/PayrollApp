using System;
using Newtonsoft.Json;

namespace Timecard.Models
{
    public class Job
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string ClientName { get; set; }
        public string JobNumber { get; set; }
        public bool IsActive { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Job other))
            {
                return false;
            }

            return Id == other.Id &&
                   Address == other.Address &&
                   ClientName == other.ClientName &&
                   JobNumber == other.JobNumber &&
                   IsActive == other.IsActive;
        }

        public override int GetHashCode()
        {
            int result = 17;

            result = 31 * result + Id.GetHashCode();
            result = 31 * result + Address.GetHashCode();
            result = 31 * result + ClientName.GetHashCode();
            result = 31 * result + JobNumber.GetHashCode();
            result = 31 * result + (IsActive ? 1 : 0);

            return result;
        }

        public static Job ShopJob()
        {
            string shop = "SHOP";
            return new Job
            {
                Id = shop,
                Address = shop,
                ClientName = shop,
                JobNumber = shop
            };
        }
    }

    class JobConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var job = (Job)value;
            if (job.Id == null)
            {
                writer.WriteValue(job.ClientName);
            }
            else
            {
                serializer.Serialize(writer, job);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                return serializer.Deserialize<Job>(reader);
            }
            else
            {
                string type = serializer.Deserialize<string>(reader);
                return new Job
                {
                    Address = type,
                    ClientName = type
                };
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Job);
        }
    }
}
