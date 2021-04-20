using Newtonsoft.Json;

namespace FriendsAndPlaces.Models
{
    public class CoordinatesResponse
    {
        [JsonProperty(PropertyName = "standort")]
        public Coordinates Coordinates { get; set; }
    }
}
