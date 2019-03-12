using System;
using Newtonsoft.Json;
using Timecard.Models;

namespace Timecard
{
    public class Item
    {
        public string Id { get; set; }
        public CostCode CostCode { get; set; }
        public DateTime JobDate { get; set; }

        [JsonConverter(typeof(JobTypeConverter))]
        public JobType JobType { get; set; }

        public Job Job { get; set; }

        [JsonConverter(typeof(TimeWorkedConverter))]
        public TimeWorked TimeWorked { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
