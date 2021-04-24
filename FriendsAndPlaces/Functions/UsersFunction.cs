using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FriendsAndPlaces.Functions
{
    public class UsersFunction
    {
        public UsersFunction()
        {

        }

        [FunctionName("CheckUsername")]
        public IActionResult CheckUsername(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/available")] HttpRequest req,
            ILogger log)
        {
            // Validate query parameter (id) -> HTTP 404

            // try get user from database

            // return OK: {erbegnis: [true|false]}

            return new OkObjectResult(null);
        }

        [FunctionName("CreateUser")]
        public IActionResult CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/new")] HttpRequest req,
            ILogger log)
        {
            // Validate Accept and Content-Type header -> HTTP 415

            // Read json body and convert to User object

            // Optional: Check if all attributes are present

            // Create user in database

            // return 200 - OK

            return new OkObjectResult(null);
        }

        [FunctionName("GetAllUsers")]
        public IActionResult GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequest req,
            ILogger log)
        {
            // Validate Accept header -> HTTP 415

            // Validate query parameters (login, session) -> HTTP 404

            // Validate session -> HTTP 403

            // Get all users from database

            // Convert users to {loginName, vorname, nachname} objects

            // return users

            return new OkObjectResult(null);
        }
    }
}

