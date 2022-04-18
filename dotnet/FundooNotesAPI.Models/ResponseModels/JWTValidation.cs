namespace FundooNotesAPI.Models.ResponseModels
{
    public class JWTValidation
    {
        public bool IsValid { get; set; }

        public string UserId { get; set; } = "";

        public string Email { get; set; } = "";


    }
}
