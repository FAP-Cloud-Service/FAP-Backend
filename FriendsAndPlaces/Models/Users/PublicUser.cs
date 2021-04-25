using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Users
{
    public class PublicUser
    {
        public string LoginName { get; set; }

        [JsonProperty(PropertyName = "vorname")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "nachname")]
        public string LastName { get; set; }
    }
}
