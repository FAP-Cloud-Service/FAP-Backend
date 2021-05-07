using FriendsAndPlaces.Configuration;
using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Helpers.GoogleGeo;
using FriendsAndPlaces.Helpers.Hashing;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(FriendsAndPlaces.Startup))]

namespace FriendsAndPlaces
{
    public class Startup : FunctionsStartup
    {
        private const string GOOGLE_API_KEY = "GOOGLE_API_KEY";
        private const string DATABASE_CONNECTION_STRING = "DATABASE_CONNECTION_STRING";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Read Google API Key from environment variables
            string googleApiKey = Environment.GetEnvironmentVariable(GOOGLE_API_KEY);
            var googleGeoConfiguration = new GoogleGeoConfiguration()
            {
                ApiKey = googleApiKey
            };

            // For local development use this connection string (connects to local storage emulator)
            // string connectionString = "UseDevelopmentStorage=true";

            // Read connection string from environment variables
            string connectionString = Environment.GetEnvironmentVariable(DATABASE_CONNECTION_STRING);
            var databaseConfiguration = new DatabaseConfiguration()
            {
                ConnectionString = connectionString
            };

            // Add services for dependency injection
            builder.Services.AddSingleton(databaseConfiguration);
            builder.Services.AddSingleton(googleGeoConfiguration);
            builder.Services.AddSingleton<IDatabaseManager, DatabaseManager>();
            builder.Services.AddSingleton<IHashManager, HashManager>();

            builder.Services.AddScoped<IGoogleGeoClient, GoogleGeoClient>();
        }
    }
}