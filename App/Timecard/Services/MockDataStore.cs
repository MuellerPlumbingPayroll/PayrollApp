using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timecard.Models;
using Timecard.Services;

namespace Timecard
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;
        List<CostCode> costCodes;
        List<Job> jobs;

        public MockDataStore()
        {
            items = new List<Item>();
            costCodes = new List<CostCode>();
            jobs = new List<Job>();
        }

        public Task<FirebaseUserInfo> AuthenticateUser(GoogleUserInfo googleUserInfo)
        {
            return null;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var _item = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var _item = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<CostCode>> GetCostCodesAsync()
        {
            return await Task.FromResult(costCodes);
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            return await Task.FromResult(jobs);
        }
    }
}
