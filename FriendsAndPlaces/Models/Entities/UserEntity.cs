using Microsoft.Azure.Cosmos.Table;

namespace FriendsAndPlaces.Models.Entities
{
    public class UserEntity : TableEntity
    {
        public UserEntity()
        {

        }

        public UserEntity(string loginName)
        {
            PartitionKey = loginName;
            RowKey = loginName;
        }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
