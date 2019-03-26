using System.Collections.Generic;

namespace Timecard.Models
{
    public class Job
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string ClientName { get; set; }
        public bool IsActive { get; set; }

        public static Job DummyJob()
        {
            return new Job()
            {
                Id = "0",
                Address = "Not Listed",
                ClientName = "Not Listed"
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
}
