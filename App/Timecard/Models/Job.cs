using System;
using System.Collections.Generic;
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

        public static Job DummyJob()
        {
            return new Job()
            {
                Id = "0",
                Address = "Not Listed",
                ClientName = "Not Listed",
                JobNumber = "0"
            };
        }

        public static List<Job> GetOtherTypeJobs()
        {
            var jobs = new List<Job>();
            foreach (var job in ProjectSettings.OtherTimeOptions)
            {
                jobs.Add(new Job()
                {
                    Address = job,
                    ClientName = job
                });
            }

            return jobs;
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
                writer.WriteValue(job);
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
