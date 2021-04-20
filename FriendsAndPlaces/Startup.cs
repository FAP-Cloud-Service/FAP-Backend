using FriendsAndPlaces.Configuration;
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

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Read Google API Key from environment variables
            string googleApiKey = Environment.GetEnvironmentVariable(GOOGLE_API_KEY);
            var googleGeoConfiguration = new GoogleGeoConfiguration()
            {
                ApiKey = googleApiKey
            };

            // Add services for dependency injection
            builder.Services.AddSingleton(googleGeoConfiguration);
            builder.Services.AddScoped<IGoogleGeoClient, GoogleGeoClient>();
        }
    }
}