using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Locations
{
    public class Location
    {
        [JsonProperty(PropertyName = "laengengrad")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "breitengrad")]
        public double Latitude { get; set; }
    }
}
