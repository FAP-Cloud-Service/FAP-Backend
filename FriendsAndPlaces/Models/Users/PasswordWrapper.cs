using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Users
{
    public class PasswordWrapper
    {
        [JsonProperty(PropertyName = "passwort")]
        public string Password { get; set; }
    }
}
