using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Models.Login;
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
    public class LoginFunction
    {
        private readonly IDatabaseManager _databaseManager;

        private const string _acceptHeaderApplicationJson = "application/json";
        private const string _contentTypeHeaderApplicationJson = "application/json";

        public LoginFunction(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [FunctionName("Login")]
        public IActionResult Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] HttpRequest req, ILogger log)
        {
            // Check if Accept: application/json is present 
            bool acceptHeaderExists = req.Headers.TryGetValue("Accept", out StringValues acceptHeaders);
            if (acceptHeaderExists && !acceptHeaders[0].Equals(_acceptHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Check if Content-Type: application/json is present 
            bool contentTypeHeaderExists = req.Headers.TryGetValue("Content-Type", out StringValues contentTypeHeaders);
            if (contentTypeHeaderExists && !contentTypeHeaders[0].Equals(_contentTypeHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read request body
            LoginRequest loginRequest;
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                loginRequest = JsonConvert.DeserializeObject<LoginRequest>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestResult();
            }

            // Check if all parameters are present
            if (string.IsNullOrWhiteSpace(loginRequest.LoginName) ||
                string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return new BadRequestResult();
            }

            // Get user from database
            var user = _databaseManager.GetUser(loginRequest.LoginName);

            // Check if user exists
            if (user == null)
            {
                return new UnauthorizedResult();
            }

            // Check if passwords match
            if (!loginRequest.Password.Equals(user.Password.Password))
            {
                return new UnauthorizedResult();
            }

            // Generate session
            Guid sessionId = Guid.NewGuid();

            // Save session in database
            bool success = _databaseManager.CreateSession(loginRequest.LoginName, sessionId.ToString());

            // Check if database action was successful
            if (!success)
            {
                return new StatusCodeResult(503);
            }

            // Return session in response body
            var loginResponse = new LoginResponse()
            {
                SessionId = sessionId.ToString()
            };

            return new OkObjectResult(JsonConvert.SerializeObject(loginResponse));
        }
    }
}
