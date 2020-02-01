using Microsoft.Extensions.Configuration;
using SmartMail.Authentication;
using SmartMail.ExceptionHandler;
using System;
using SmartMail.Graph;
namespace SmartMail
{
    class Program
    {
        static IConfigurationRoot LoadAppSettings()
        {
            var appConfig = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            // Check for required settings
            if (string.IsNullOrEmpty(appConfig["appId"]) ||
                string.IsNullOrEmpty(appConfig["scopes"]))
            {
                return null;
            }

            return appConfig;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var appConfig = LoadAppSettings();

            if (appConfig == null)
            {
                Logging.Log("Missing or invalid appsettings.json...exiting", 'e');
                return;
            }

            var appId = appConfig["appId"];
            var scopesString = appConfig["scopes"];
            var scopes = scopesString.Split(';');

            // Initialize the auth provider with values from appsettings.json
            var authProvider = new MSGraphDeviceCodeAuthProvider(appId, scopes);
            if (authProvider != null)//if user logs in
            {
                // Request a token to sign in the user
                var accessToken = authProvider.GetAccessToken().Result;
                if(accessToken != null)
                    Logging.Log($"Got access token", 'i');
                // Initialize Graph client
                GraphHelper.Initialize(authProvider);

                // Get signed in user
                var user = GraphHelper.GetMeAsync().Result;
                Console.WriteLine($"Welcome {user.DisplayName}!\n");

            }

        }
    }
}
