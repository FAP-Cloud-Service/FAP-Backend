using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.GoogleGeo
{
    public class GoogleGeoLocation
    {
        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public double Longitude { get; set; }
    }
}