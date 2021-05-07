using System.Security.Cryptography;
using System.Text;

namespace FriendsAndPlaces.Helpers.Hashing
{
    public class HashManager : IHashManager
    {
        public string HashString(string input)
        {
            using (SHA512 shaM = new SHA512Managed())
            {
                byte[] hash = shaM.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Encoding.UTF8.GetString(hash);
            }
        }
    }
}
