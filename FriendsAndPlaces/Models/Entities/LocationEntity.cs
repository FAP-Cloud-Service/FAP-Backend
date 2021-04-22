using Microsoft.Azure.Cosmos.Table;

namespace FriendsAndPlaces.Models.Entities
{
    public class LocationEntity : TableEntity
    {
        public LocationEntity()
        {

        }

        public LocationEntity(string loginName)
        {
            PartitionKey = loginName;
            RowKey = loginName;
        }

        public string LoginName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
