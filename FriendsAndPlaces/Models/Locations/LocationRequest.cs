using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Locations
{
    class LocationRequest
    {
        public string LoginName { get; set; }
        [JsonProperty(PropertyName = "sitzung")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "laengengrad")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "breitengrad")]
        public double Latitude { get; set; }
    }
}
