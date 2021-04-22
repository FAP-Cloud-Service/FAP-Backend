using FriendsAndPlaces.Configuration;
using FriendsAndPlaces.Helpers.Database;
using FriendsAndPlaces.Helpers.GoogleGeo;
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

            // Read connection string from environment variables
            //string connectionString = Environment.GetEnvironmentVariable(DATABASE_CONNECTION_STRING);
            string connectionString = "UseDevelopmentStorage=true";
            var databaseConfiguration = new DatabaseConfiguration()
            {
                ConnectionString = connectionString
            };

            // Add services for dependency injection
            builder.Services.AddSingleton(databaseConfiguration);
            builder.Services.AddSingleton(googleGeoConfiguration);
            builder.Services.AddSingleton<IDatabaseManager, DatabaseManager>();

            builder.Services.AddScoped<IGoogleGeoClient, GoogleGeoClient>();
        }
    }
}