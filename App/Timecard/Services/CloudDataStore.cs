using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugin.Connectivity;
using Timecard.iOS;
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

        private readonly JsonSerializerSettings _serializerSettings;
        private FirebaseUserInfo _firebaseUserInfo;

        public CloudDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            items = new List<Item>();
            costCodes = new List<CostCode>();
            jobs = new List<Job>();

            _serializerSettings = new JsonSerializerSettings
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
                var firebaseUserInfo = ReadFirebaseUserInfoFromDevice();
                if (firebaseUserInfo != null)
                {
                    await SetAuthenticationHeader();

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
                await SetAuthenticationHeader();

                var json = await client.GetStringAsync($"api/item/{id}");
                return await Task.Run(() => JsonConvert.DeserializeObject<Item>(json));
            }

            return null;
        }

        public async Task<string> AddItemAsync(Item item)
        {
            var firebaseUserInfo = ReadFirebaseUserInfoFromDevice();

            if (item == null || firebaseUserInfo == null || !CrossConnectivity.Current.IsConnected)
                return null;

            await SetAuthenticationHeader();

            var serializedItem = JsonConvert.SerializeObject(item, _serializerSettings);
            var response = await client.PostAsync($"entry/{firebaseUserInfo.Id}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var firebaseUserInfo = ReadFirebaseUserInfoFromDevice();

            if (item == null || item.Id == null || firebaseUserInfo == null || !CrossConnectivity.Current.IsConnected)
                return false;

            await SetAuthenticationHeader();

            var serializedItem = JsonConvert.SerializeObject(item, _serializerSettings);
            var response = await client.PostAsync($"entry/{firebaseUserInfo.Id}/{item.Id}", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var firebaseUserInfo = ReadFirebaseUserInfoFromDevice();

            if (string.IsNullOrEmpty(id) || firebaseUserInfo == null || !CrossConnectivity.Current.IsConnected)
                return false;

            await SetAuthenticationHeader();

            var response = await client.DeleteAsync($"entry/{firebaseUserInfo.Id}/{id}");
            
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<CostCode>> GetCostCodesAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await SetAuthenticationHeader();

                var json = await client.GetStringAsync("cost-code");
                costCodes = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<CostCode>>(json));
            }
            
            return costCodes;
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await SetAuthenticationHeader();

                var json = await client.GetStringAsync("jobs");
                jobs = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Job>>(json));
            }

            return jobs;
        }

        public async Task<bool> SubmitTimecardAsync(TimecardSubmission timecardSubmission)
        {
            var firebaseUserInfo = ReadFirebaseUserInfoFromDevice();

            if (timecardSubmission == null || firebaseUserInfo == null || !CrossConnectivity.Current.IsConnected)
                return false;

            await SetAuthenticationHeader();

            var serializedSubmission = JsonConvert.SerializeObject(timecardSubmission, _serializerSettings);
            var response = await client.PostAsync($"submit/{firebaseUserInfo.Id}", new StringContent(serializedSubmission, Encoding.UTF8, "application/json"));
            
            return response.IsSuccessStatusCode;
        }

        // Method to reduce the number of reads from the device's storage
        private FirebaseUserInfo ReadFirebaseUserInfoFromDevice()
        {
            if (_firebaseUserInfo == null)
            {
                _firebaseUserInfo = FirebaseUserInfo.ReadFromDevice().Result;
            }

            return _firebaseUserInfo;
        }

        private async Task SetAuthenticationHeader()
        {
            string firebaseToken = await FirebaseManager.Auth.CurrentUser.GetIdTokenAsync(false);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firebaseToken);
        }
    }
}
