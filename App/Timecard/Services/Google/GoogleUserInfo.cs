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

        [JsonProperty(PropertyName = "given_name")]
        public string GivenName { get; set; } = "Employee";

        public async void SaveToDevice()
        {
            await SecureStorage.SetAsync("Email", Email);
            await SecureStorage.SetAsync("GivenName", GivenName);
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
                        GivenName = await SecureStorage.GetAsync("GivenName"),
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
            SecureStorage.Remove("Email");
            SecureStorage.Remove("GivenName");
        }
    }
}
