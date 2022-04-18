using FundooNotesAPI.Models.RequestModels;
using FundooNotesAPI.Models.ResponseModels;
using Microsoft.AspNetCore.Http;

namespace FundooNotesAPI.Shared.Interfaces
{
    public interface IJWTService
    {
        string GetJWT(string userId, string email);

        JWTValidation ValidateJWT(HttpRequest req);
    }
}
