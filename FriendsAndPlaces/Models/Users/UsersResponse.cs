using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Users
{
    public class UsersResponse
    {
        [JsonProperty(PropertyName = "benutzerliste")]
        public PublicUser[] Users { get; set; }
    }
}
