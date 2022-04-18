using Newtonsoft.Json;

namespace FundooNotesAPI.Models.RequestModels
{
    public class FundooUser
    {
        [JsonProperty("id")]
        public string UserId { get; set; } = "";


        [JsonProperty("firstName")]
        public string FirstName { get; set; } = "";


        [JsonProperty("lastName")]
        public string LastName { get; set; } = "";


        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; } = "";


        [JsonProperty("email")]
        public string Email { get; set; } = "";


        [JsonProperty("password")]
        public string Password { get; set; } = "";


        [JsonProperty("address")]
        public string Address { get; set; } = "";


        [JsonProperty("registeredDateTime")]
        public string RegisteredAt { get; set; } = "";
    }
}
