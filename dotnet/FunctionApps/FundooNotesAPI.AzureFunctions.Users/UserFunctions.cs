using FundooNotesAPI.Models.RequestModels;
using FundooNotesAPI.Models.ResponseModels;
using FundooNotesAPI.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace FundooNotesAPI.AzureFunctions.Users
{
    public class UserFunctions
    {
        private readonly ILogger<UserFunctions> _logger;
        private readonly IUserService _userService;
        private readonly IJWTService _jWTService;

        public UserFunctions(ILogger<UserFunctions> log, IUserService userService, IJWTService jWTService)
        {
            _logger = log;
            _userService = userService;
            _jWTService = jWTService;
        }

        [FunctionName("UserRegistration")]
        [OpenApiOperation(operationId: "UserRegistration", tags: new[] { "Registration" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "email", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(FundooUser), Required = true, Description = "New user details.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(FundooUser), Description = "The OK response")]
        public async Task<IActionResult> UserRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<FundooUser>(requestBody);
            var response = await _userService.UserRegistration(data);

            return new OkObjectResult(response);
        }

        [FunctionName("UserLogin")]
        [OpenApiOperation(operationId: "UserLogin", tags: new[] { "Login" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "email", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LoginCredentials), Required = true, Description = "New user details.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResponse), Description = "The OK response")]
        public async Task<IActionResult> UserLogin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/login")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<LoginCredentials>(requestBody);
            var response = await _userService.UserLogin(data);

            return new OkObjectResult(response);
        }

        [FunctionName("ResetPassword")]
        [OpenApiOperation(operationId: "ResetPassword", tags: new[] { "Reset Password" })]
        [OpenApiSecurity("BearerToken", SecuritySchemeType.ApiKey, Name = "token", In = OpenApiSecurityLocationType.Header)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LoginCredentials), Required = true, Description = "New user details.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> ResetPassword(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users")] HttpRequest req)
        {
            var authResponse = _jWTService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<LoginCredentials>(requestBody);
            var response = await _userService.UserLogin(data);

            return new OkObjectResult(response);
        }
    }
}

