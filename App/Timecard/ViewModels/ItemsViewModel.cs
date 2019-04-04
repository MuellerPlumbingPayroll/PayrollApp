using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Timecard.Models;

namespace Timecard
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command AddItemCommand { get; set; }
        public Command UpdateItemCommand { get; set; }
        public Command DeleteItemCommand { get; set; }

        public Command LoadJobsCommand { get; set; }
        public Command LoadCostCodesCommand { get; set; }

        public Dictionary<JobType, List<Job>> Jobs { get; set; }
        public Dictionary<JobType, List<CostCode>> CostCodes { get; set; }

        public ItemsViewModel()
        {
            Title = "History";
            Items = new ObservableCollection<Item>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command<Item>(async (Item item) => await AddItem(item));
            UpdateItemCommand = new Command<Item>(async (Item item) => await UpdateItem(item));
            DeleteItemCommand = new Command<Item>(async (Item item) => await DeleteItem(item));

            LoadItemsCommand.Execute(null);

            LoadJobsCommand = new Command(async () => await ExecuteLoadJobsCommand());
            LoadCostCodesCommand = new Command(async () => await ExecuteLoadCostCodesCommand());

            Jobs = new Dictionary<JobType, List<Job>>
            {
                {JobType.Construction, new List<Job>()},
                {JobType.Service, new List<Job>{new Job()} },
                {JobType.Other, Job.GetOtherTypeJobs()}
            };

            LoadJobsCommand.Execute(null);

            CostCodes = new Dictionary<JobType, List<CostCode>>();
            LoadCostCodesCommand.Execute(null);
        }

        public List<Item>[] GetItemSections()
        {
            var numberSections = 7 * ProjectSettings.NumberWeeksInPayPeriod;
            var sections = new List<Item>[numberSections];

            var dayInPayPeriod = ProjectSettings.GetStartOfCurrentPayPeriod();

            for (var i = 0; i < numberSections; i++)
            {
                sections[i] = Items.ToList()
                    .Where(item => item.JobDate.ToLocalTime().DayOfWeek.Equals(dayInPayPeriod.DayOfWeek))
                    .ToList();

                dayInPayPeriod = dayInPayPeriod.AddDays(1);
            }

            return sections;
        }

        /// <summary>
        /// Calculate the numbers the hours worked on a given day.
        /// If no day is provided, the method calculates the total  
        /// number of hours worked, regardless of the day.
        /// </summary>
        /// <returns>The hours worked on the specified day.</returns>
        /// <param name="day">Day.</param>
        public float NumberHoursWorkedOnDay(DayOfWeek? day = null)
        {
            var timeEntries = Items.ToList();

            if (day != null) 
            {
                timeEntries = timeEntries
                    .Where(item => (bool)item.JobDate.ToLocalTime().DayOfWeek.Equals(day))
                    .ToList();
            }

            float hoursWorked = 0;
            foreach (var item in timeEntries)
            {
                hoursWorked += item.TimeWorked.AsFloat();
            }

            return hoursWorked;
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var items = await DataStore.GetItemsAsync(true);
                Items.Clear();

                foreach (var item in items)
                {
                    Items.Add(item);
                }

                Debug.WriteLine($"Successfully loaded {Items.Count} items.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> AddItem(Item item)
        {
            string itemId = await DataStore.AddItemAsync(item);

            if (itemId != null)
            {
                Debug.WriteLine($"Successfully added new item: {itemId}.");

                item.Id = itemId;
                Items.Add(item);

                return true;
            }
            else
            {
                Debug.WriteLine("Failed to add new item.");
                return false;
            }
        }

        public async Task<bool> UpdateItem(Item item)
        {
            var elementToRemove = -1;
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Id == item.Id)
                {
                    elementToRemove = i;
                    break;
                }
            }

            if (elementToRemove >= 0)
            {
                bool success = await DataStore.UpdateItemAsync(item);

                if (success)
                {
                    Debug.WriteLine($"Successfully updated item: {item.Id}.");
                    Items.RemoveAt(elementToRemove);
                    Items.Insert(elementToRemove, item);
                    return true;
                }
                else
                {
                    Debug.WriteLine($"Failed to update item: {item.Id}.");
                }
            }

            return false;
        }

        public async Task<bool> DeleteItem(Item item)
        {
            bool success = await DataStore.DeleteItemAsync(item.Id);

            if (success)
            {
                Debug.WriteLine($"Successfully deleted item: {item.Id}.");
                Items.Remove(item);
                return true;
            }
            else
            {
                Debug.WriteLine($"Failed to delete item: {item.Id}.");
                return false;
            }
        }

        async Task ExecuteLoadCostCodesCommand()
        {
            try
            {
                var costCodes = await DataStore.GetCostCodesAsync();

                CostCodes.Clear();

                foreach (var costCode in costCodes)
                {
                    JobType key;
                    if (costCode.CodeGroup == CostCode.PlumbingCodeGroup)
                        key = JobType.Construction;
                    else if (costCode.CodeGroup == CostCode.ServiceCodeGroup)
                        key = JobType.Service;
                    else
                        key = JobType.Other;
                        
                    if (CostCodes.ContainsKey(key)) 
                    {
                        CostCodes[key].Add(costCode);
                    }
                    else
                    {
                        CostCodes.Add(key, new List<CostCode> {
                            costCode
                        });
                    }
                }

                Debug.WriteLine($"Successfully added {CostCodes.Count} cost codes.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        async Task ExecuteLoadJobsCommand()
        {
            try
            {
                var jobs = await DataStore.GetJobsAsync();

                Jobs[JobType.Construction].Clear();

                foreach (var job in jobs)
                {
                    Jobs[JobType.Construction].Add(job);
                }

                if (Jobs[JobType.Construction].Count == 0)
                {
                    Debug.WriteLine($"Failed to find any jobs. Adding a dummy job.");
                    Jobs[JobType.Construction].Add(Job.DummyJob());
                }
                else
                {
                    Debug.WriteLine($"Successfully added {Jobs[JobType.Construction].Count} construction jobs.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
