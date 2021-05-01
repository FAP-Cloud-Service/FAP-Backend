using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Logout
{
    public class LogoutResponse
    {
        [JsonProperty(PropertyName = "ergebnis")]
        public bool Result { get; set; }
    }
}


