using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Plugin.Connectivity;
using Timecard.Models;

namespace Timecard
{
    public class CloudDataStore : IDataStore<Item>
    {
        HttpClient client;
        IEnumerable<Item> items;
        IEnumerable<CostCode> costCodes;
        IEnumerable<Job> jobs;

        public CloudDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            items = new List<Item>();
            costCodes = new List<CostCode>();
            jobs = new List<Job>();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync($"api/item");
                items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Item>>(json));
            }

            return items;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            if (id != null && CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync($"api/item/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<Item>(json));
            }

            return null;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            if (item == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PostAsync($"api/item", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item == null || item.Id == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await client.PutAsync(new Uri($"api/item/{item.Id}"), byteContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (string.IsNullOrEmpty(id) && !CrossConnectivity.Current.IsConnected)
                return false;

            var response = await client.DeleteAsync($"api/item/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<CostCode>> GetCostCodesAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync("cost-code");
                costCodes = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<CostCode>>(json));
            }
            
            return costCodes;
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var json = await client.GetStringAsync("jobs");
                jobs = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Job>>(json));
            }

            return jobs;
        }
    }
}
