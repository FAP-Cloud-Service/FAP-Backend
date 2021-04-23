using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Users
{
    public class EmailWrapper
    {
        [JsonProperty(PropertyName = "adresse")]
        public string Email { get; set; }
    }
}
