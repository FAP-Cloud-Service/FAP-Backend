using Microsoft.Azure.Cosmos.Table;

namespace FriendsAndPlaces.Models.Entities
{
    public class SessionEntity : TableEntity
    {
        public SessionEntity()
        {

        }

        public SessionEntity(string loginName)
        {
            PartitionKey = loginName;
            RowKey = loginName;
        }

        public string LoginName { get; set; }

        public string SessionId { get; set; }
    }
}
