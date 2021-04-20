using FriendsAndPlaces.Helpers.GoogleGeo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace FriendsAndPlaces.Functions
{
    public class CoordinatesFunction
    {
        private readonly IGoogleGeoClient _googleGeoClient;

        private const string _acceptHeaderApplicationJson = "application/json";
        private const string _contentTypeHeaderApplicationJson = "application/json";

        public CoordinatesFunction(IGoogleGeoClient googleGeoClient)
        {
            _googleGeoClient = googleGeoClient;
        }

        // Function URI: /api/coordinates?land=Deutschland&plz=46397&ort=Bocholt&strasse=M�nsterstrasse 265

        [FunctionName("Coordinates")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("Incoming request.");

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

            // Read query parameters
            string city = req.Query["ort"];
            string street = req.Query["strasse"];
            string country = req.Query["land"];
            string postalCode = req.Query["plz"];

            // Check if all parameters are present
            if (city == null ||
                street == null ||
                country == null ||
                postalCode == null)
            {
                return new BadRequestResult();
            }

            // Call Google Geocoding API
            var coordinatesResponse = _googleGeoClient.GetCoordinatesForAddress(country, postalCode, city, street);

            // Check if location was found
            if (coordinatesResponse != null)
            {
                return new OkObjectResult(JsonConvert.SerializeObject(coordinatesResponse));
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
