using FriendsAndPlaces.Models;

namespace FriendsAndPlaces.Helpers.GoogleGeo
{
    public interface IGoogleGeoClient
    {
        public CoordinatesResponse GetCoordinatesForAddress(string country, string postalCode, string city, string street);
    }
}
