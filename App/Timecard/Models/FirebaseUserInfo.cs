using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Timecard.Models
{
    public class FirebaseUserInfo
    {
        public string Id { get; set; }

        public async void SaveToDevice()
        {
            await SecureStorage.SetAsync("UserId", Id);
        }

        public static async Task<FirebaseUserInfo> ReadFromDevice()
        {
            try
            {
                var id = await SecureStorage.GetAsync("UserId");

                if (id != null)
                {
                    return new FirebaseUserInfo()
                    {
                        Id = id
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
            SecureStorage.Remove("UserId");
        }
    }
}
