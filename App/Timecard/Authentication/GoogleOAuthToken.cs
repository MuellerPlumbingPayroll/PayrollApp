using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Timecard.Authentication
{
    public class GoogleOAuthToken
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }

        public async void SaveToDevice()
        {
            await SecureStorage.SetAsync("TokenType", TokenType);
            await SecureStorage.SetAsync("AccessToken", AccessToken);
            await SecureStorage.SetAsync("IdToken", IdToken);
        }

        public static async Task<GoogleOAuthToken> ReadFromDevice()
        {
            try
            {
                var tokenType = await SecureStorage.GetAsync("TokenType");

                if (tokenType != null)
                {
                    return new GoogleOAuthToken()
                    {
                        TokenType = tokenType,
                        AccessToken = await SecureStorage.GetAsync("AccessToken"),
                        IdToken = await SecureStorage.GetAsync("IdToken")
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
            SecureStorage.Remove("TokenType");
            SecureStorage.Remove("AccessToken");
            SecureStorage.Remove("IdToken");
        }
    }
}
