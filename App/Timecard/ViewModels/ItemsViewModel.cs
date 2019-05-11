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
                {JobType.Other, new List<Job>{Job.ShopJob()}}
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
        /// <returns>The hours worked on the specified date.</returns>
        /// <param name="date">Day.</param>
        public float NumberHoursWorkedOnDay(DateTime? date = null)
        {
            var timeEntries = Items.ToList();

            if (date != null)
            {
                var dayOfWeek = ((DateTime)date).DayOfWeek;
                var day = ((DateTime)date).Day;

                timeEntries = timeEntries
                    .Where(item => (bool)item.JobDate.ToLocalTime().DayOfWeek.Equals(dayOfWeek) &&
                                   (bool)item.JobDate.ToLocalTime().Day.Equals(day))
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
                    if (costCode.CodeGroup == CostCode.PlumbingCodeGroup || costCode.CodeGroup == CostCode.HeatingCodeGroup)
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

                Debug.WriteLine($"Successfully added {CostCodes[JobType.Construction].Count} construction cost codes.");
                Debug.WriteLine($"Successfully added {CostCodes[JobType.Service].Count} service cost codes.");
                Debug.WriteLine($"Successfully added {CostCodes[JobType.Other].Count} shop cost codes.");
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

                if (Jobs.ContainsKey(JobType.Construction))
                {
                    Debug.WriteLine($"Successfully added {Jobs[JobType.Construction].Count} construction jobs.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public DateTime GetStartOfPayPeriod()
        {
            if (Items.Count > 0) {
                var sortedItems = Items.OrderBy(o => o.JobDate).ToList();
                var earliestItem = sortedItems[0];
                
                var startCurrent = ProjectSettings.GetStartOfCurrentPayPeriod();

                int result = DateTime.Compare(earliestItem.JobDate.ToLocalTime().Date, startCurrent.Date);
                if (result < 0)
                {
                    return startCurrent.AddDays(-7);
                }
            }

            return ProjectSettings.GetStartOfCurrentPayPeriod();
        }

        public DateTime GetEndOfPayPeriod()
        {
            return GetStartOfPayPeriod().AddDays(6);
        }

        public DateTime GetInitialDate()
        {
            if (DateTime.Compare(GetStartOfPayPeriod().Date, ProjectSettings.GetStartOfCurrentPayPeriod().Date) < 0)
            {
                return DateTime.Now.AddDays(-7);
            }

            return DateTime.Now;
        }
    }
}
