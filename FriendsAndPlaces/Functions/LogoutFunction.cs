using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Models.Logout;
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
    public class LogoutFunction
    {
        private readonly IDatabaseManager _databaseManager;

        private const string _acceptHeaderApplicationJson = "application/json";
        private const string _contentTypeHeaderApplicationJson = "application/json";

        public LogoutFunction(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [FunctionName("Logout")]
        public IActionResult Logout(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "logout")] HttpRequest req, ILogger log)
        {
            // Check headers -> HTTP 415
            bool acceptHeaderExists = req.Headers.TryGetValue("Accept", out StringValues acceptHeaders);
            if (acceptHeaderExists && !acceptHeaders[0].Equals(_acceptHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            bool contentTypeHeaderExists = req.Headers.TryGetValue("Content-Type", out StringValues contentTypeHeaders);
            if (contentTypeHeaderExists && !contentTypeHeaders[0].Equals(_contentTypeHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read request body
            LogoutRequest logoutRequest;
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                logoutRequest = JsonConvert.DeserializeObject<LogoutRequest>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestResult();
            }

            // Check body -> HTTP 400
            if (string.IsNullOrWhiteSpace(logoutRequest.LoginName) ||
                string.IsNullOrWhiteSpace(logoutRequest.SessionId))
            {
                return new BadRequestResult();
            }

            // Delete session from database
            _databaseManager.DeleteSession(logoutRequest.LoginName, logoutRequest.SessionId);

            // Return 200 no matter what response the database gives
            // => That way we don't reveal whether loginName or session are valid

            var logoutResponse = new LogoutResponse()
            {
                Result = true
            };

            return new OkObjectResult(JsonConvert.SerializeObject(logoutResponse));
        }
    }
}
