using FundooNotesAPI.Models.RequestModels;
using FundooNotesAPI.Models.ResponseModels;

namespace FundooNotesAPI.Shared.Interfaces
{
    public interface IUserService
    {
        Task<List<FundooUser>> GetUsers();

        Task<FundooUser> UserRegistration(FundooUser newUserDetails);

        Task<LoginResponse> UserLogin(LoginCredentials userLoginDetails);

        Task<FundooUser> ResetPassword(FundooUser updatedUserDetails);

        Task<FundooUser> ForgotPassword(string userEmail);
    }
}
