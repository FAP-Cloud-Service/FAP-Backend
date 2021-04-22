using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FriendsAndPlaces.Functions
{
    public class LoginFunction
    {
        [FunctionName("Login")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Check headers -> HTTP 415

            // Check parameters -> HTTP 400

            // Get user from database

            // If user does not exist -> HTTP 401

            // Compare passwords

            // Passwords do not match -> HTTP 401

            // Generate session

            // Save session in database

            // Return session in response body

            return new OkObjectResult(null);
        }
    }
}

