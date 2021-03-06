using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FriendsAndPlaces.Functions
{
    public class UsersFunction
    {
        private readonly IDatabaseManager _databaseManager;

        private const string _acceptHeaderApplicationJson = "application/json";
        private const string _contentTypeHeaderApplicationJson = "application/json";

        public UsersFunction(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [FunctionName("CheckUsername")]
        public IActionResult CheckUsername(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/available")] HttpRequest req)
        {
            // Get query parameter
            string loginName = req.Query["id"];

            // Validate parameters
            if (string.IsNullOrWhiteSpace(loginName))
            {
                return new BadRequestResult();
            }

            // Get user from database
            var user = _databaseManager.GetUser(loginName);

            if (user == null)
            {
                // LoginName is available
                return new OkObjectResult(new AvailabilityResponse() { Result = true });
            }
            else
            {
                // LoginName is already taken
                return new OkObjectResult(new AvailabilityResponse() { Result = false });
            }
        }

        [FunctionName("CreateUser")]
        public IActionResult CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/new")] HttpRequest req, ILogger log)
        {
            // Check if Content-Type: application/json is present 
            bool contentTypeHeaderExists = req.Headers.TryGetValue("Content-Type", out StringValues contentTypeHeaders);
            if (contentTypeHeaderExists && !contentTypeHeaders[0].Equals(_contentTypeHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read request body
            User user;
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                user = JsonConvert.DeserializeObject<User>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestResult();
            }

            // Check if key properties are missing
            if (user == null ||
                user.Password == null ||
                string.IsNullOrEmpty(user.LoginName) ||
                string.IsNullOrEmpty(user.Password.Password))
            {
                return new BadRequestResult();
            }

            // Check if user already exists
            var existingUser = _databaseManager.GetUser(user.LoginName);

            if (existingUser != null)
            {
                // LoginName is already taken
                return new BadRequestObjectResult($"Username {user.LoginName} is already taken.");
            }

            // Create user in database
            var success = _databaseManager.CreateUser(user);

            // Check if database creation was successful
            if (!success)
            {
                return new StatusCodeResult(503);
            }

            return new CreatedResult($"{user.LoginName}", null);
        }

        [FunctionName("GetAllUsers")]
        public IActionResult GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequest req)
        {
            // Check if Accept: application/json is present 
            bool acceptHeaderExists = req.Headers.TryGetValue("Accept", out StringValues acceptHeaders);
            if (acceptHeaderExists && !acceptHeaders[0].Equals(_acceptHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Get query parameters
            string loginName = req.Query["login"];
            string sessionId = req.Query["session"];

            // Validate parameters
            if (string.IsNullOrWhiteSpace(loginName) ||
                string.IsNullOrWhiteSpace(sessionId))
            {
                return new BadRequestResult();
            }

            // Get session from database
            string savedSession = _databaseManager.GetSession(loginName);

            // Session for user does not exist
            if (string.IsNullOrWhiteSpace(savedSession))
            {
                return new UnauthorizedResult();
            }

            // Sessions do not match
            if (!sessionId.Equals(savedSession))
            {
                return new UnauthorizedResult();
            }

            // Get all users from database
            var users = _databaseManager.GetAllUsers();

            if (users == null)
            {
                return new NoContentResult();
            }

            // Convert users to publicUser objects
            var publicUsers = new List<PublicUser>();
            foreach (var user in users)
            {
                publicUsers.Add(new PublicUser()
                {
                    LoginName = user.LoginName,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }

            // Create return object
            var usersResponse = new UsersResponse()
            {
                Users = publicUsers.ToArray()
            };

            return new OkObjectResult(usersResponse);
        }
    }
}
