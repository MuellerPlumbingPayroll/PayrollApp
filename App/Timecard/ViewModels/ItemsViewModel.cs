using System;
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
        public Dictionary<string, List<string>> JobDescriptions { get; set; }

        public ItemsViewModel()
        {
            Title = "History";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command<Item>(async (Item item) => await AddItem(item));
            UpdateItemCommand = new Command<Item>(async (Item item) => await UpdateItem(item));

            JobDescriptions = new Dictionary<string, List<string>>
            {
                // TODO: Instead of adding a dummy value, call the api to get the actual jobs
                {JobType.Construction, new List<string>{"testing"}},
                {JobType.Service, new List<string>()},
                {JobType.Other, new List<string>(ProjectSettings.OtherTimeOptions)}
            };
        }

        public float NumberHoursWorkedToday()
        {
            if (Items.Count == 0)
                LoadItemsCommand.Execute(null);

            var today = DateTime.Now.ToString(ProjectSettings.DateFormat);

            float hoursWorked = 0;
            foreach (var item in Items)
            {
                hoursWorked += 10;
                if (item.JobDate == today)
                {
                    hoursWorked += float.Parse(item.HoursWorked);
                }
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
                Items.Add(item);
                await DataStore.UpdateItemAsync(item);
            }
        }
    }
}
