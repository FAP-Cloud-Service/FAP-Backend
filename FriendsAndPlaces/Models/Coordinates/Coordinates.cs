using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Coordinates
{
    public class Coordinates
    {
        [JsonProperty(PropertyName = "laengengrad")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "breitengrad")]
        public double Latitude { get; set; }
    }
}
