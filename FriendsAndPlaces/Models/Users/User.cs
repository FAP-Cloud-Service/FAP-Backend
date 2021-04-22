using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Users
{
    public class User
    {

        public string LoginName { get; set; }

        [JsonProperty(PropertyName = "passwort")]
        public PasswordWrapper Password { get; set; }

        [JsonProperty(PropertyName = "vorname")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "nachname")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "strasse")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "ort")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "plz")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "land")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "telefon")]
        public string Phone { get; set; }

        public EmailWrapper Email { get; set; }
    }
}
