using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Timecard.Services
{
    public class GoogleService
    {
        public async Task<GoogleUserInfo> GetUserInfoAsync(string tokenType, string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

            var json = await httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v1/userinfo?alt=json");
            var userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(json);

            return userInfo;
        }
    }
}
