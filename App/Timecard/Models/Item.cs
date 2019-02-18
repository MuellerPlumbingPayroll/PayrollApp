using System;
using Timecard.Exceptions;
using Timecard.Models;

using System.Text.RegularExpressions;

namespace Timecard
{
    public class Item
    {
        public string Id { get; set; }
        public DateTime JobDate { get; set; }
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

        public void Clean()
        {
            switch (JobType)
            {
                case Models.JobType.Construction:
                case Models.JobType.Service:
                    if (string.IsNullOrEmpty(CostCode)) 
                        throw new InvalidItemException("Cost code must have a value.");
                    break;
                case Models.JobType.Other:
                    CostCode = string.Empty;
                    break;
            }

            if (string.IsNullOrEmpty(JobDescription))
            {
                throw new InvalidItemException("Job description must have a value.");
            }
            else // Validate number of hours worked
            {
                HoursWorked = HoursWorked?.Trim();

                // Hours worked is supposed to be a string in the format hours:minutes
                // Minutes can only be in increments of 15

                string regexPattern = "\\A\\d{1,2}:\\d{2}\\z";
                var match = Regex.Match(HoursWorked, regexPattern);
                if (!match.Success)
                    throw new InvalidItemException("Invalid time format.");

                var times = HoursWorked.Split(":");
                var isValidHours = int.TryParse(times[0], out int hours);
                var isValidMinutes = int.TryParse(times[1], out int minutes);

                if (!isValidHours || !isValidMinutes)
                    throw new InvalidItemException("Invalid time format.");
                else if (hours == 0 && minutes == 0)
                    throw new InvalidItemException("Time must be greater than zero.");
                else if (hours > ProjectSettings.MaxNumberHoursInWorkDay)
                    throw new InvalidItemException(string.Format(
                        $"Invalid number of hours. The value must be between 0 and {ProjectSettings.MaxNumberHoursInWorkDay}."));
                else if (minutes > 45 || minutes % 15 != 0)
                    throw new InvalidItemException("Invalid number of minutes. The value must be 0, 15, 30, or 45.");

                var time = new Decimal(hours + (float)minutes / 60);
                HoursWorked = string.Format($"{Decimal.Round(time, 2)}");
                System.Diagnostics.Debug.WriteLine("hours worked is: " + HoursWorked);
            }
        }
    }
}
