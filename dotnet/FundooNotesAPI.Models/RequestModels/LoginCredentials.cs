using Newtonsoft.Json;

namespace FundooNotesAPI.Models.RequestModels
{
    public class LoginCredentials
    {
        [JsonProperty("email")]
        public string Email { get; set; } = "";

        [JsonProperty("password")]
        public string Password { get; set; } = "";
    }
}
