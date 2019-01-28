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
        public string JobType { get; set; }
        public string JobDescription { get; set; }
        public string CostCode { get; set; }
        public string HoursWorked { get; set; }

        public Item()
        {
            this.Id = Guid.NewGuid().ToString("N");
        }

        public string CleanAndValidate()
        {
            this.JobDescription = this.JobDescription?.Trim();
            this.HoursWorked = this.HoursWorked?.Trim();
            return this.Validate();
        }

        private string Validate()
        {
            if (!Array.Exists(Models.JobType.Types, element => element == JobType))
            {
                return "Invalid Job Type.";
            }
            else if (string.IsNullOrEmpty(this.JobDescription))
            {
                return "Job Description must have a value";
            }
            else if (string.IsNullOrEmpty(this.HoursWorked))
            {
                return "Hours must have a value";
            }
            else
            {
                var isValid = float.TryParse(this.HoursWorked, out float n);

                if (isValid && n > 0 && n <= MAX_NUM_HOURS_WORKED)
                {
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
