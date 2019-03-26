using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugin.Connectivity;
using Timecard.Models;
using Timecard.Services;

namespace Timecard
{
    public class CloudDataStore : IDataStore<Item>
    {
        HttpClient client;
        IEnumerable<Item> items;
        IEnumerable<CostCode> costCodes;
        IEnumerable<Job> jobs;

        private JsonSerializerSettings serializerSettings;

        public CloudDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            items = new List<Item>();
            costCodes = new List<CostCode>();
            jobs = new List<Job>();

            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<FirebaseUserInfo> AuthenticateUser(GoogleUserInfo googleUserInfo)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var response = await client.GetAsync($"authenticate/{googleUserInfo.Email}");

                if (response.IsSuccessStatusCode)
                {
                    string id = await response.Content.ReadAsStringAsync();
                    return new FirebaseUserInfo()
                    {
                        Id = id
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh && CrossConnectivity.Current.IsConnected)
            {
                var firebaseUserInfo = FirebaseUserInfo.ReadFromDevice().Result;
                if (firebaseUserInfo != null)
                {
                    var json = await client.GetStringAsync($"entries/{firebaseUserInfo.Id}");
                    items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Item>>(json));
                }
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
            var firebaseUserInfo = FirebaseUserInfo.ReadFromDevice().Result;

            if (item == null || firebaseUserInfo == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item, serializerSettings);
            var response = await client.PostAsync($"entry/{firebaseUserInfo.Id}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                string id = await response.Content.ReadAsStringAsync();
                // TODO: Use this ID
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var firebaseUserInfo = FirebaseUserInfo.ReadFromDevice().Result;

            if (item == null || item.Id == null || firebaseUserInfo == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(item, serializerSettings);
            var response = await client.PostAsync($"entry/{firebaseUserInfo.Id}/{item.Id}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

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
