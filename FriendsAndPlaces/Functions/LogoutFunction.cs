using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Models.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FriendsAndPlaces.Functions
{
    public class LogoutFunction
    {
        private readonly IDatabaseManager _databaseManager;

        private const string _acceptHeaderApplicationJson = "application/json";
        private const string _contentTypeHeaderApplicationJson = "application/json";

        public LoginFunction(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [FunctionName("Logout")]
        public async Task<IActionResult> Logout(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Incoming logout request.");

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

            string requestBody =  new StreamReader(req.Body).ReadToEndAsync().Result;
            var logoutRequest = JsonConvert.DeserializeObject<LogoutRequest>(requestBody);


            // Check body -> HTTP 400
            if (logoutRequest.LoginName == null ||
                logoutRequest.SessionId == null)
            {
                return new BadRequestResult();
            }
            
            // Delete session from database

            

            _databaseManager.DeleteSession(logoutRequest.LoginName, logoutRequest.SessionId.ToString());

            // Return 200 no matter what response the database gives
            // => That way we dont reveal whether loginName or session are valid
            // => e.g. "Success! Session revoken (if it existed)"

            var logoutResponse = new LogoutResponse()
            {
                Message = @"Success! Session revoken (if it existed)";
                SessionId = logoutRequest.SessionId.ToString();
            }

            return new OkObjectResult(JsonConvert.SerializeObject(logoutResponse));
        }
    }
}

