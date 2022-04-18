using Newtonsoft.Json;

namespace FundooNotesAPI.Models.RequestModels
{
    public class ResetPassword
    {
        [JsonProperty("password")]
        public string Password { get; set; } = "";


        [JsonProperty("password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
