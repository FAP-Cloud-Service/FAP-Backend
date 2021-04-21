using FriendsAndPlaces.Configuration;
using FriendsAndPlaces.Models;
using FriendsAndPlaces.Models.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace FriendsAndPlaces.Helpers.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        private const string USERS_TABLE_NAME = "users";
        private const string SESSIONS_TABLE_NAME = "sessions";
        private const string LOCATIONS_TABLE_NAME = "locations";

        public DatabaseManager(DatabaseConfiguration configuration)
        {
            _cloudStorageAccount = CreateStorageAccountFromConnectionString(configuration.ConnectionString);
        }

        #region Users
        #endregion Users

        #region Sessions

        public bool CreateSession(string loginName, string sessionId)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(SESSIONS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                // Create table
                cloudTable.Create();
            }

            // Insert data
            try
            {
                // Create table entity
                var sessionEntity = new SessionEntity(loginName)
                {
                    SessionId = sessionId
                };

                // Create the InsertOrReplace table operation
                var tableOperation = TableOperation.InsertOrReplace(sessionEntity);

                // Execute the operation
                var result = ExecuteOperation(cloudTable, tableOperation);

                if (result.HttpStatusCode == 200)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public string GetSession(string loginName)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(SESSIONS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                Console.WriteLine($"Table {SESSIONS_TABLE_NAME} does not exist");
                return null;
            }

            // Get data
            try
            {
                var tableOperation = TableOperation.Retrieve<SessionEntity>(loginName, loginName);

                var result = ExecuteOperation(cloudTable, tableOperation);
                
                var sessionEntity = result.Result as SessionEntity;

                if (sessionEntity != null)
                {
                    return sessionEntity.SessionId;
                }

                return null;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                return null; ;
            }
        }

        public void DeleteSession(string loginName, string sessionId)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(SESSIONS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                Console.WriteLine($"Table {SESSIONS_TABLE_NAME} does not exist");
                return;
            }

            // Delete data
            try
            {
                // Get entity from database
                var tableOperation = TableOperation.Retrieve<SessionEntity>(loginName, loginName);

                var result = ExecuteOperation(cloudTable, tableOperation);

                var sessionEntity = result.Result as SessionEntity;

                if (sessionEntity == null)
                {
                    Console.WriteLine("Session does not exist. Does not have to be deleted");
                    return;
                }

                tableOperation = TableOperation.Delete(sessionEntity);
                
                result = ExecuteOperation(cloudTable, tableOperation);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        #endregion Session

        #region Locations

        public bool SetLocation(string loginName, double longitude, double latitude)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(LOCATIONS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                // Create table
                cloudTable.Create();
            }

            // Insert data
            try
            {
                // Create table entity
                var locationEntity = new LocationEntity(loginName)
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                // Create the InsertOrReplace table operation
                var tableOperation = TableOperation.InsertOrReplace(locationEntity);

                // Execute the operation
                var result = ExecuteOperation(cloudTable, tableOperation);

                if (result.HttpStatusCode == 200)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Coordinates GetLocation(string loginName)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(LOCATIONS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                Console.WriteLine($"Table {SESSIONS_TABLE_NAME} does not exist");
                return null;
            }

            // Get data
            try
            {
                var tableOperation = TableOperation.Retrieve<LocationEntity>(loginName, loginName);

                var result = ExecuteOperation(cloudTable, tableOperation);

                var locationEntity = result.Result as LocationEntity;

                if (locationEntity != null)
                {
                    return new Coordinates()
                    {
                        Latitude = locationEntity.Latitude,
                        Longitude = locationEntity.Longitude
                    };
                }

                return null;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        #endregion Locations

        private TableResult ExecuteOperation(CloudTable cloudTable, TableOperation tableOperation)
        {
            var result = cloudTable.ExecuteAsync(tableOperation).Result;

            if (result.RequestCharge.HasValue)
            {
                Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
            }

            Console.WriteLine($"HttpStatusCode: {result.HttpStatusCode}");

            return result;
        }

        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the settings.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the settings.");
                throw;
            }

            return storageAccount;
        }
    }
}
