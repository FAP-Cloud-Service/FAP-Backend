using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Users
{
    public class AvailabilityResponse
    {
        [JsonProperty(PropertyName = "ergebnis")]
        public bool Result { get; set; }
    }
}
