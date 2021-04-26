using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Locations
{
    public class LocationRequest
    {
        public string LoginName { get; set; }

        [JsonProperty(PropertyName = "sitzung")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "standort")]
        public Location Location { get; set; }

    }
}
