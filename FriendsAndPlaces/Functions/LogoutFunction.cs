using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FriendsAndPlaces.Functions
{
    public static class LogoutFunction
    {
        [FunctionName("Logout")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Check headers -> HTTP 415

            // Check body -> HTTP 400

            // Delete session from database

            // Return 200 no matter what response the database gives
            // => That way we dont reveal whether loginName or session are valid
            // => e.g. "Success! Session revoken (if it existed)"

            return new OkObjectResult(null);
        }
    }
}

