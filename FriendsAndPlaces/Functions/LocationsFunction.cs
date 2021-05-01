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
using System;
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
        public IActionResult SetLocation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "setlocation")] HttpRequest req, ILogger log)
        {
            // Check if Content-Type: application/json is present 
            bool contentTypeHeaderExists = req.Headers.TryGetValue("Content-Type", out StringValues contentTypeHeaders);
            if (contentTypeHeaderExists && !contentTypeHeaders[0].Equals(_contentTypeHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read request body
            LocationRequest locationRequest;
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
                locationRequest = JsonConvert.DeserializeObject<LocationRequest>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestResult();
            }

            // Check parameters -> HTTP 400
            // Check if all parameters are present
            if (string.IsNullOrWhiteSpace(locationRequest.LoginName) ||
                string.IsNullOrWhiteSpace(locationRequest.SessionId) ||
                locationRequest.Location == null)
            {
                return new BadRequestResult();
            }

            // Get user and session from database
            var user = _databaseManager.GetUser(locationRequest.LoginName);
            var session = _databaseManager.GetSession(locationRequest.LoginName);

            // If user does not exist or session is invalid -> HTTP 401
            if (user == null ||
                string.IsNullOrWhiteSpace(session) ||
                locationRequest.SessionId != session)
            {
                return new UnauthorizedResult();
            }

            // Save location in database
            bool success = _databaseManager.SetLocation(locationRequest.LoginName, locationRequest.Location.Longitude, locationRequest.Location.Latitude);

            // Write in Database failed -> HTTP 503
            if (!success)
            {
                return new StatusCodeResult(503);
            }

            return new OkResult();
        }

        [FunctionName("GetLocation")]
        public IActionResult GetLocation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "location")] HttpRequest req)
        {
            // Check headers -> HTTP 415
            // Check if Accept: application/json is present 
            bool acceptHeaderExists = req.Headers.TryGetValue("Accept", out StringValues acceptHeaders);
            if (acceptHeaderExists && !acceptHeaders[0].Equals(_acceptHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read request parameters
            string loginName = req.Query["login"];
            string sessionId = req.Query["session"];
            string userId = req.Query["id"];

            // Check parameters -> HTTP 400
            // Check if all parameters are present
            if (string.IsNullOrWhiteSpace(loginName) ||
                string.IsNullOrWhiteSpace(sessionId) ||
                string.IsNullOrWhiteSpace(userId))
            {
                return new BadRequestResult();
            }

            // Get user and session from database
            var session = _databaseManager.GetSession(loginName);

            // If user does not exist or user is not logged in with correct SessionId -> HTTP 401
            if (string.IsNullOrWhiteSpace(session))
            {
                return new UnauthorizedResult();
            }

            // Get Location from Database
            var location = _databaseManager.GetLocation(userId);

            if (location == null)
            {
                return new NoContentResult();
            }

            //Formatting of Response to Coordinates
            Coordinates locationResponse = new Coordinates
            {
                Longitude = location.Longitude,
                Latitude = location.Latitude
            };

            //Response with Coordinates
            return new OkObjectResult(JsonConvert.SerializeObject(locationResponse));
        }
    }
}
