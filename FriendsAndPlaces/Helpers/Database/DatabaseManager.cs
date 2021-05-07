using FriendsAndPlaces.Configuration;
using FriendsAndPlaces.Helpers.Hashing;
using FriendsAndPlaces.Models.Coordinates;
using FriendsAndPlaces.Models.Entities;
using FriendsAndPlaces.Models.Users;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace FriendsAndPlaces.Helpers.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly IHashManager _hashManager;
        private readonly CloudStorageAccount _cloudStorageAccount;

        private const string USERS_TABLE_NAME = "users";
        private const string SESSIONS_TABLE_NAME = "sessions";
        private const string LOCATIONS_TABLE_NAME = "locations";

        public DatabaseManager(IHashManager hashManager, DatabaseConfiguration configuration)
        {
            _hashManager = hashManager;
            _cloudStorageAccount = CreateStorageAccountFromConnectionString(configuration.ConnectionString);
        }

        #region Users

        public bool CreateUser(User user)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(USERS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                // Create table
                cloudTable.Create();
            }

            // Insert data
            try
            {
                // Create table entity
                var userEntity = new UserEntity(user.LoginName)
                {
                    LoginName = user.LoginName,
                    Password = _hashManager.HashString(user.Password.Password),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Street = user.Street,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    Country = user.Country,
                    Phone = user.Phone,
                    Email = user.Email.Email
                };

                // Create the InsertOrReplace table operation
                var tableOperation = TableOperation.InsertOrReplace(userEntity);

                // Execute the operation
                var result = ExecuteOperation(cloudTable, tableOperation);

                if (result.HttpStatusCode == 204)
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

        public User GetUser(string loginName)
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(USERS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                Console.WriteLine($"Table {USERS_TABLE_NAME} does not exist");
                return null;
            }

            // Get data
            try
            {
                var tableOperation = TableOperation.Retrieve<UserEntity>(loginName, loginName);

                var result = ExecuteOperation(cloudTable, tableOperation);

                var userEntity = result.Result as UserEntity;

                if (userEntity != null)
                {
                    return new User()
                    {
                        LoginName = userEntity.LoginName,
                        Password = new PasswordWrapper()
                        {
                            Password = userEntity.Password
                        },
                        FirstName = userEntity.FirstName,
                        LastName = userEntity.LastName,
                        Street = userEntity.Street,
                        PostalCode = userEntity.PostalCode,
                        City = userEntity.City,
                        Country = userEntity.Country,
                        Phone = userEntity.Phone,
                        Email = new EmailWrapper()
                        {
                            Email = userEntity.Email
                        }
                    };
                }

                return null;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                return null; ;
            }
        }

        public User[] GetAllUsers()
        {
            var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var cloudTable = cloudTableClient.GetTableReference(USERS_TABLE_NAME);
            if (!cloudTable.Exists())
            {
                Console.WriteLine($"Table {USERS_TABLE_NAME} does not exist");
                return null;
            }

            // Get data
            try
            {
                // From: https://stackoverflow.com/questions/23940246/how-to-get-all-rows-in-azure-table-storage-in-c

                TableContinuationToken token = null;
                var userEntities = new List<UserEntity>();
                do
                {
                    var queryResult = cloudTable.ExecuteQuerySegmented(new TableQuery<UserEntity>(), token);
                    userEntities.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);

                if (userEntities.Count > 0)
                {
                    // Transform UserEntity objects to User objects

                    var users = new List<User>();

                    foreach (var userEntity in userEntities)
                    {
                        users.Add(new User()
                        {
                            LoginName = userEntity.LoginName,
                            Password = new PasswordWrapper()
                            {
                                Password = userEntity.Password
                            },
                            FirstName = userEntity.FirstName,
                            LastName = userEntity.LastName,
                            Street = userEntity.Street,
                            PostalCode = userEntity.PostalCode,
                            City = userEntity.City,
                            Country = userEntity.Country,
                            Phone = userEntity.Phone,
                            Email = new EmailWrapper()
                            {
                                Email = userEntity.Email
                            }
                        });
                    }

                    return users.ToArray();
                }

                return new User[0];
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                return null; ;
            }
        }

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
                    LoginName = loginName,
                    SessionId = sessionId
                };

                // Create the InsertOrReplace table operation
                var tableOperation = TableOperation.InsertOrReplace(sessionEntity);

                // Execute the operation
                var result = ExecuteOperation(cloudTable, tableOperation);

                if (result.HttpStatusCode == 204)
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
                    LoginName = loginName,
                    Latitude = latitude,
                    Longitude = longitude
                };

                // Create the InsertOrReplace table operation
                var tableOperation = TableOperation.InsertOrReplace(locationEntity);

                // Execute the operation
                var result = ExecuteOperation(cloudTable, tableOperation);

                if (result.HttpStatusCode == 204)
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
                Console.WriteLine("Request Charge of operation: " + result.RequestCharge);
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
