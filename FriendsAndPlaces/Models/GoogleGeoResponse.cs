namespace FriendsAndPlaces.Models
{
    public class GoogleGeoResponse
    {
        public string Status { get; set; }

        public GoogleGeoResult[] Results { get; set; }
    }
}
