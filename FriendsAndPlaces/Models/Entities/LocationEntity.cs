using Microsoft.Azure.Cosmos.Table;

namespace FriendsAndPlaces.Models.Entities
{
    public class LocationEntity : TableEntity
    {
        public LocationEntity(string loginName)
        {
            PartitionKey = loginName;
            RowKey = loginName;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
