using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Timecard.Services
{
    public class GoogleUserInfo
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "given_name")]
        public string GivenName { get; set; }

        [JsonProperty(PropertyName = "family_name")]
        public string FamilyName { get; set; }

        public async void SaveToDevice()
        {
            await SecureStorage.SetAsync("Email", Email);
            await SecureStorage.SetAsync("Name", Name);
            await SecureStorage.SetAsync("GivenName", GivenName);
            await SecureStorage.SetAsync("FamilyName", FamilyName);
        }

        public static async Task<GoogleUserInfo> ReadFromDevice()
        {
            try
            {
                var email = await SecureStorage.GetAsync("Email");

                if (email != null)
                {
                    return new GoogleUserInfo()
                    {
                        Email = email,
                        Name = await SecureStorage.GetAsync("Name"),
                        GivenName = await SecureStorage.GetAsync("GivenName"),
                        FamilyName = await SecureStorage.GetAsync("FamilyName")
                    };
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void RemoveFromDevice()
        {
            SecureStorage.RemoveAll();
        }
    }
}
