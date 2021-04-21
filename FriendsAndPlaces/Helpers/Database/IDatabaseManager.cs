using FriendsAndPlaces.Models;

namespace FriendsAndPlaces.Helpers.Database
{
    public interface IDatabaseManager
    {
        //public void CreateUser();

        //public string GetUser(string loginName);

        public bool CreateSession(string loginName, string sessionId);

        public string GetSession(string loginName);

        public void DeleteSession(string loginName, string sessionId);

        public bool SetLocation(string loginName, double longitude, double latitude);

        public Coordinates GetLocation(string loginName);
    }
}
