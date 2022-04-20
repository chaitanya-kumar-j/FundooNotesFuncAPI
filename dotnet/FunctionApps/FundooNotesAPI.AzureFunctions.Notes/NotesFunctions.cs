using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FundooNotesAPI.Models.RequestModels;
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

namespace FundooNotesAPI.AzureFunctions.Notes
{
    public class NotesFunctions
    {
        private readonly ILogger<NotesFunctions> _logger;
        private readonly IJWTService _jwtService;
        private readonly INotesService _notesService;

        public NotesFunctions(ILogger<NotesFunctions> log, IJWTService jWTService, INotesService notesService)
        {
            _logger = log;
            _jwtService = jWTService;
            _notesService = notesService;
        }

        [FunctionName("GetAllNotes")]
        [OpenApiOperation(operationId: "GetAllNotes", tags: new[] { "Get All Notes" })]
        [OpenApiSecurity("JWT Bearer Token", SecuritySchemeType.ApiKey, Name = "token", In = OpenApiSecurityLocationType.Header)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",bodyType: typeof(List<FundooNote>), Description = "The OK response")]
        public async Task<IActionResult> GetAllNotes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notes")] HttpRequest req)
        {
            var authResponse = _jwtService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }
            var response = await _notesService.GetAllFundooNotes(authResponse.Email);
            return new OkObjectResult(response);
        }


        [FunctionName("GetNoteById")]
        [OpenApiOperation(operationId: "GetNoteById", tags: new[] { "Get Note By Id" })]
        [OpenApiSecurity("JWT Bearer Token", SecuritySchemeType.ApiKey, Name = "token", In = OpenApiSecurityLocationType.Header)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The id parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(FundooNote), Description = "The OK response")]
        public async Task<IActionResult> GetNoteById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "notes/{id}")] HttpRequest req, string id)
        {
            var authResponse = _jwtService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }
            var response = await _notesService.GetFundooNoteById(authResponse.Email, id);
            return new OkObjectResult(response);
        }


        [FunctionName("CreateNote")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Create Note" })]
        [OpenApiSecurity("JWT Bearer Token", SecuritySchemeType.ApiKey, Name = "token", In = OpenApiSecurityLocationType.Header)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(FundooNote), Required = true, Description = "New note details.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(FundooNote), Description = "The OK response")]
        public async Task<IActionResult> CreateNote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "notes")] HttpRequest req)
        {
            var authResponse = _jwtService.ValidateJWT(req);
            if (!authResponse.IsValid)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<FundooNote>(requestBody);

            var response = _notesService.CreateFundooNote(authResponse.Email, data);
            return new OkObjectResult(response);
        }
    }
}

