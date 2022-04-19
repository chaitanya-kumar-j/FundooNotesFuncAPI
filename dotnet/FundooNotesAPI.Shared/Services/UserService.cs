using FundooNotesAPI.Models.RequestModels;
using FundooNotesAPI.Models.ResponseModels;
using FundooNotesAPI.Shared.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

namespace FundooNotesAPI.Shared.Services
{

    public class UserService : IUserService
    {

        private readonly IJWTService _jWTService;
        ISettingsService _settingsService;

        // Cosmos DocDB API database
        private string _docDbEndpointUri;
        private string _docDbPrimaryKey;
        private string _docDbDatabaseName;

        // Doc DB Collections
        private string _docDbDigitalMainCollectionName;

        private static CosmosClient _docDbSingletonClient;
        private readonly Container _cosmosContainer;


        
        public UserService(IJWTService jWTService, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _jWTService = jWTService;
            _docDbEndpointUri = _settingsService.GetDocDbEndpointUri();
            _docDbPrimaryKey = _settingsService.GetDocDbApiKey();

            _docDbDatabaseName = _settingsService.GetDocDbFundooNotesDatabaseName();
            _docDbDigitalMainCollectionName = _settingsService.GetDocDbMainCollectionName();
            _docDbSingletonClient = new CosmosClient(_settingsService.GetDocDbEndpointUri(), settingsService.GetDocDbApiKey());
            _cosmosContainer = _docDbSingletonClient.GetContainer(_docDbDatabaseName, _docDbDigitalMainCollectionName);
            /*_cosmosContainer = new Lazy<Task<Container>>(async () =>
            {
                var cosmos = new CosmosClient(settingsService.GetDocDbEndpointUri(), settingsService.GetDocDbApiKey());
                var db = cosmos.GetDatabase(settingsService.GetDocDbFundooNotesDatabaseName());
                //TODO: Hardcoded partition key field here
                return await db.CreateContainerIfNotExistsAsync(settingsService.GetDocDbMainCollectionName(),"/email");
            });*/
        }

        //**** PRIVATE METHODS ****//
        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
        // private Task<Container> GetCosmosContainer() => _cosmosContainer.Value;

        #region Get All Users
        public async Task<List<FundooUser>> GetUsers()
        {
            try
            {
                if (string.IsNullOrEmpty(_docDbDigitalMainCollectionName))
                    throw new Exception("No Digital Main collection defined!");

                using (var query = _cosmosContainer.GetItemLinqQueryable<FundooUser>()
                                .OrderByDescending(e => e.RegisteredAt)
                                .ToFeedIterator())
                {
                    List<FundooUser> allDocuments = new List<FundooUser>();
                    while (query.HasMoreResults)
                    {
                        var queryResult = await query.ReadNextAsync();

                        allDocuments.AddRange(queryResult.ToList());
                    }

                    return allDocuments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region User Registration
        public async Task<FundooUser> UserRegistration(FundooUser newUserDetails)
        {
            if (string.IsNullOrEmpty(newUserDetails.UserId))
            {
                newUserDetails.RegisteredAt = Convert.ToString(DateTime.Now);
                newUserDetails.UserId = Guid.NewGuid().ToString();
            }

            try
            {
                using (var response = _cosmosContainer.CreateItemAsync(newUserDetails, new PartitionKey(newUserDetails.Email)))
                {
                    return response.Result.Resource;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        #endregion

        #region User Login
        public async Task<LoginResponse> UserLogin(LoginCredentials userLoginDetails)
        {
            LoginResponse loginResponse = new LoginResponse();
            try
            {
                if (string.IsNullOrEmpty(userLoginDetails.Email))
                    throw new Exception("Email is null or empty string!!");

                if (string.IsNullOrEmpty(_docDbDigitalMainCollectionName))
                    throw new Exception("No Digital Main collection defined!");

                var user = _cosmosContainer.GetItemLinqQueryable<FundooUser>(true)
                     .Where(u => u.Email == userLoginDetails.Email)
                     .AsEnumerable()
                     .FirstOrDefault();
                if (user == null)
                {
                    return null;
                }
                loginResponse.UserDetails = user;
                loginResponse.token = _jWTService.GetJWT(loginResponse.UserDetails.UserId, loginResponse.UserDetails.Email);
                return loginResponse;
            }
            catch (Exception ex)
            {
                // Detect a `Resource Not Found` exception...do not treat it as error
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) && ex.InnerException.Message.IndexOf("Resource Not Found") != -1)
                {
                    return null;
                }
                else
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }
        #endregion

        #region Reset Password
        public Task<FundooUser> ResetPassword(FundooUser updatedUserDetails)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Forgot Password
        public Task<FundooUser> ForgotPassword(string userEmail)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}