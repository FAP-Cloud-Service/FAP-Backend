using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Coordinates
{
    public class CoordinatesResponse
    {
        [JsonProperty(PropertyName = "standort")]
        public Coordinates Coordinates { get; set; }
    }
}
