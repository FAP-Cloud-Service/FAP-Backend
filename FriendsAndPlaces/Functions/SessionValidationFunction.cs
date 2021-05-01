using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Models.SessionValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.IO;

namespace FriendsAndPlaces.Functions
{
    public class SessionValidationFunction
    {
        private readonly IDatabaseManager _databaseManager;

        private const string _contentTypeHeaderApplicationJson = "application/json";

        public SessionValidationFunction(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [FunctionName("SessionValidation")]
        public IActionResult ValidateSession(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/validate")] HttpRequest req, ILogger log)
        {
            // Check if Content-Type: application/json is present 
            bool contentTypeHeaderExists = req.Headers.TryGetValue("Content-Type", out StringValues contentTypeHeaders);
            if (contentTypeHeaderExists && !contentTypeHeaders[0].Equals(_contentTypeHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read request body
            SessionValidationRequest sessionValidationRequest;
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                sessionValidationRequest = JsonConvert.DeserializeObject<SessionValidationRequest>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestResult();
            }

            // Check if all parameters are present
            if (string.IsNullOrWhiteSpace(sessionValidationRequest.LoginName) ||
                string.IsNullOrWhiteSpace(sessionValidationRequest.SessionId))
            {
                return new BadRequestResult();
            }

            // Get user and session from database
            var user = _databaseManager.GetUser(sessionValidationRequest.LoginName);
            var session = _databaseManager.GetSession(sessionValidationRequest.LoginName);

            // If user does not exist or session is invalid -> HTTP 401
            if (user == null ||
                string.IsNullOrWhiteSpace(session) ||
                sessionValidationRequest.SessionId != session)
            {
                return new UnauthorizedResult();
            }

            // Session is valid
            return new OkResult();
        }
    }
}
