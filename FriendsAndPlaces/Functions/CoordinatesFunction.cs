using FriendsAndPlaces.Helpers.GoogleGeo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Primitives;

namespace FriendsAndPlaces.Functions
{
    public class CoordinatesFunction
    {
        private readonly IGoogleGeoClient _googleGeoClient;

        private const string _acceptHeaderApplicationJson = "application/json";

        public CoordinatesFunction(IGoogleGeoClient googleGeoClient)
        {
            _googleGeoClient = googleGeoClient;
        }

        [FunctionName("Coordinates")]
        public IActionResult GetCoordinates(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "coordinates")] HttpRequest req)
        {
            // Check if Accept: application/json is present 
            bool acceptHeaderExists = req.Headers.TryGetValue("Accept", out StringValues acceptHeaders);
            if (acceptHeaderExists && !acceptHeaders[0].Equals(_acceptHeaderApplicationJson))
            {
                return new UnsupportedMediaTypeResult();
            }

            // Read query parameters
            string city = req.Query["ort"];
            string street = req.Query["strasse"];
            string country = req.Query["land"];
            string postalCode = req.Query["plz"];

            // Check if all parameters are present
            if (string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(street) ||
                string.IsNullOrWhiteSpace(country) ||
                string.IsNullOrWhiteSpace(postalCode))
            {
                return new BadRequestResult();
            }

            // Call Google Geocoding API
            var coordinatesResponse = _googleGeoClient.GetCoordinatesForAddress(country, postalCode, city, street);

            // Check if location was found
            if (coordinatesResponse == null)
            {
                return new NoContentResult();
            }

            return new OkObjectResult(coordinatesResponse);
        }
    }
}
