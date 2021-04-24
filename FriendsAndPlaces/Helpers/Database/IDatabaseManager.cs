using FriendsAndPlaces.Models.Coordinates;
using FriendsAndPlaces.Models.Users;

namespace FriendsAndPlaces.Helpers.Database
{
    public interface IDatabaseManager
    {
        public bool CreateUser(User user);

        public User GetUser(string loginName);

        public User[] GetAllUsers();

        public bool CreateSession(string loginName, string sessionId);

        public string GetSession(string loginName);

        public void DeleteSession(string loginName, string sessionId);

        public bool SetLocation(string loginName, double longitude, double latitude);

        public Coordinates GetLocation(string loginName);
    }
}
