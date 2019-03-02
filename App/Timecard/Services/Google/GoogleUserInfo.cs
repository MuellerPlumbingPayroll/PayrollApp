using Newtonsoft.Json;

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
    }
}
