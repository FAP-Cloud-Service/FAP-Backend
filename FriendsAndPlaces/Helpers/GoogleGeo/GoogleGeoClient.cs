using FriendsAndPlaces.Configuration;
using FriendsAndPlaces.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace FriendsAndPlaces.Helpers.GoogleGeo
{
    public class GoogleGeoClient : IGoogleGeoClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _googleApiKey;

        private const string _baseUri = "https://maps.googleapis.com/maps/api/geocode/json";

        public GoogleGeoClient(HttpClient httpClient, GoogleGeoConfiguration configuration)
        {
            _httpClient = httpClient;
            _googleApiKey = configuration.ApiKey;
        }

        public CoordinatesResponse GetCoordinatesForAddress(string country, string postalCode, string city, string street)
        {
            // Create request message
            var requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_baseUri}?address={street} {postalCode} {city} {country}&key={_googleApiKey}"),
                Method = HttpMethod.Get
            };

            // Send request
            var response = _httpClient.SendAsync(requestMessage).Result;

            // Check response status code
            if (response.IsSuccessStatusCode)
            {
                var googleGeoResponse = JsonConvert.DeserializeObject<GoogleGeoResponse>(response.Content.ReadAsStringAsync().Result);

                // Check Google status code
                if (googleGeoResponse.Status.Equals("OK"))
                {
                    // Read latitude and longitude and return response object
                    var coordinatesReponse = new CoordinatesResponse()
                    {
                        Coordinates = new Coordinates()
                        {
                            Latitude = googleGeoResponse.Results[0].Geometry.Location.Latitude,
                            Longitude = googleGeoResponse.Results[0].Geometry.Location.Longitude
                        }
                    };

                    return coordinatesReponse;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
