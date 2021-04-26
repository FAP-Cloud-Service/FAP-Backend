using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Locations
{
    public class Location
    {
        [JsonProperty(PropertyName = "breitengrad")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "laengengrad")]
        public double Longitude { get; set; }

    }
}
