using FriendsAndPlaces.Models.Users;
using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Login
{
    public class LoginRequest
    {
        public string LoginName { get; set; }

        [JsonProperty(PropertyName = "passwort")]
        public PasswordWrapper Password { get; set; }
    }
}
