using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Models.Coordinates;
using FriendsAndPlaces.Models.Locations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.IO;

namespace FriendsAndPlaces.Functions
{
    public class LocationsFunction
    {
        private readonly IDatabaseManager _databaseManager;

        private const string _acceptHeaderApplicationJson = "application/json";
        private const string _contentTypeHeaderApplicationJson = "application/json";

        public LocationsFunction(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [FunctionName("SetLocation")]
        public ActionResult SetLocation(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Incoming SetLocation request.");

            // Check headers -> HTTP 415
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

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
            var locationRequest = JsonConvert.DeserializeObject<LocationRequest>(requestBody);

            // Check parameters -> HTTP 400
            // Check if all parameters are present
            if (locationRequest.LoginName == null ||
                locationRequest.SessionId == null ||
                locationRequest.Latitude == 0 ||
                locationRequest.Longitude == 0)
            {
                return new BadRequestResult();
            }

            // Get user and session from database
            var user = _databaseManager.GetUser(locationRequest.LoginName);
            var session = _databaseManager.GetSession(locationRequest.SessionId);

            // If user does not exist or user is not logged in with correct SessionId -> HTTP 401
            if (user == null || session == null)
            {
                return new UnauthorizedResult();
            }

            // Save location in database
            bool success = _databaseManager.SetLocation(locationRequest.LoginName, locationRequest.Longitude, locationRequest.Latitude);

            // Write in Database failed -> HTTP 503
            /*if (!success)
            {
                return new StatusCodeResult(503);
            }
            */
            return new CreatedResult("Database", new {LoginName = locationRequest.LoginName, Laengengrad = locationRequest.Longitude, Breitengrad = locationRequest.Latitude });
            
        }

        [FunctionName("GetLocation")]
        public ActionResult GetLocation(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Incoming GetLocation request.");

            // Check headers -> HTTP 415
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
            string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
            var locationRequest = JsonConvert.DeserializeObject<LocationRequest>(requestBody);

            // Check parameters -> HTTP 400
            // Check if all parameters are present
            if (locationRequest.LoginName == null ||
                locationRequest.SessionId == null ||
                locationRequest.Latitude == 0 ||
                locationRequest.Longitude == 0)
            {
                return new BadRequestResult();
            }

            // Get user and session from database
            var user = _databaseManager.GetUser(locationRequest.LoginName);
            var session = _databaseManager.GetSession(locationRequest.SessionId);

            // If user does not exist or user is not logged in with correct SessionId -> HTTP 401
            if (user == null || session == null)
            {
                return new UnauthorizedResult();
            }

            // Get Location from Database
            var location = _databaseManager.GetLocation(locationRequest.LoginName);

            if (location == null)
            {
                return new BadRequestResult();
            }

            //Formatting of Response to Coordinates
            Coordinates locationResponse = new Coordinates
            {
                Longitude = location.Longitude,
                Latitude = location.Latitude
            };

            //Response with Coordinates
            return new OkObjectResult(JsonConvert.SerializeObject(locationResponse, Formatting.Indented));

        }
    }
}
