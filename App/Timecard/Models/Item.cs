using System;
using System.Linq;
using Timecard.Models;

namespace Timecard
{
    public class Item
    {
        private readonly static float MAX_NUM_HOURS_WORKED = 18;

        public string Id { get; set; }
        public string JobDate { get; set; }
        public string ConstructionJobId { get; set; }
        public string JobDescription { get; set; }
        public string JobType { get; set; }
        public string CostCode { get; set; }
        public string HoursWorked { get; set; }
        public long TimeCreated { get; set; }
        public long TimeUpdated { get; set; }
        public string LatitudeCreated { get; set; }
        public string LongitudeCreated { get; set; }
        public string LatitudeUpdated { get; set; }
        public string LongitudeUpdated { get; set; }

        public Item()
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.TimeCreated = (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            this.TimeUpdated = this.TimeCreated;
        }

        public string CleanAndValidate()
        {
            this.JobDescription = this.JobDescription?.Trim();
            return this.Validate();
        }

        private string Validate()
        {
            if (this.JobType != Models.JobType.Other)
            {
                if (string.IsNullOrEmpty(this.CostCode))
                {
                    return "Cost code must have a value";
                }
            }

            if (string.IsNullOrEmpty(this.JobDescription))
            {
                return "Job Description must have a value";
            }
            else // Validate number of hours worked
            {
                var isValid = float.TryParse(this.HoursWorked, out float n);

                if (isValid && n > 0 && n <= MAX_NUM_HOURS_WORKED)
                {
                    System.Diagnostics.Debug.WriteLine("the time is: " + n.ToString("0.##"));
                    this.HoursWorked = n.ToString("0.##");
                    return null;
                }
                else
                {
                    return string.Format($"Invalid number of hours. The value must be between 0 and {MAX_NUM_HOURS_WORKED}");
                }
            }
        }
    }
}
