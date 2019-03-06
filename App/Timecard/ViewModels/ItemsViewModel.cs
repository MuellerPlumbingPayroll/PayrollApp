﻿using System;
using System.Linq;
using System.Collections.ObjectModel;

using System.Collections.Generic;
using System.Diagnostics;
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

        public Dictionary<string, List<string>> Jobs { get; set; }
        public Dictionary<string, List<CostCode>> CostCodes { get; set; }

        public ItemsViewModel()
        {
            Title = "History";
            Items = new ObservableCollection<Item>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command<Item>(async (Item item) => await AddItem(item));
            UpdateItemCommand = new Command<Item>(async (Item item) => await UpdateItem(item));
            DeleteItemCommand = new Command<Item>(async (Item item) => await DeleteItem(item));

            LoadJobsCommand = new Command(async () => await ExecuteLoadJobsCommand());
            LoadCostCodesCommand = new Command(async () => await ExecuteLoadCostCodesCommand());

            Jobs = new Dictionary<string, List<string>>
            {
                {JobType.Construction, new List<string>()},
                {JobType.Service, new List<string>()},
                {JobType.Other, new List<string>(ProjectSettings.OtherTimeOptions)}
            };

            LoadJobsCommand.Execute(null);

            CostCodes = new Dictionary<string, List<CostCode>>();
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
                    .Where(item => item.JobDate.Equals(dayInPayPeriod.ToString(ProjectSettings.DateFormat)))
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
            if (Items.Count == 0)
                LoadItemsCommand.Execute(null);

            var timeEntries = Items.ToList();

            if (day != null) 
            {
                timeEntries = timeEntries
                    .Where(item => (bool)ProjectSettings.LocalDateFromString(item.JobDate)?.DayOfWeek.Equals(day))
                    .ToList();
            }

            float hoursWorked = 0;
            foreach (var item in timeEntries)
            {
                hoursWorked += float.Parse(item.HoursWorked);
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
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
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

        async Task AddItem(Item item)
        {
            Items.Add(item);
            await DataStore.AddItemAsync(item);
        }

        async Task UpdateItem(Item item)
        {
            item.TimeUpdated = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

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
                Items.RemoveAt(elementToRemove);
                Items.Insert(elementToRemove, item);
                await DataStore.UpdateItemAsync(item);
            }
        }

        async Task DeleteItem(Item item)
        {
            Items.Remove(item);
            await DataStore.DeleteItemAsync(item.Id);
        }

        async Task ExecuteLoadCostCodesCommand()
        {
            try
            {
                CostCodes.Clear();

                var costCodes = await DataStore.GetCostCodesAsync();
                foreach (var costCode in costCodes)
                {
                    var costCodeGroup = costCode.CodeGroup;
                    if (CostCodes.ContainsKey(costCodeGroup)) 
                    {
                        CostCodes[costCodeGroup].Add(costCode);
                    }
                    else
                    {
                        CostCodes.Add(costCodeGroup, new List<CostCode> {
                            costCode
                        });
                    }
                }
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
                Jobs[JobType.Construction].Clear();

                var jobs = await DataStore.GetJobsAsync();
                foreach (var job in jobs)
                {
                    Jobs[JobType.Construction].Add(job.Address);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
