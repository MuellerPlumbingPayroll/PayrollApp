using System;
using Timecard.Models;

namespace Timecard
{
    public class Item
    {
        public string Id { get; set; }
        public CostCode CostCode { get; set; }
        public DateTime JobDate { get; set; }
        public JobType JobType { get; set; }
        public Job Job { get; set; }
        public TimeWorked TimeWorked { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
