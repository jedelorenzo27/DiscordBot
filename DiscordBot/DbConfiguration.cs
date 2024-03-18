using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class DbConfiguration
    {

        public static IConfiguration BuildConfiguration()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static string GetDatabaseConnectionString(IConfiguration configuration)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environmentName == "Development")
            {
                return configuration.GetConnectionString("DefaultConnection");
            }
            else 
            {
                var keyVaultBaseUrl = configuration["AzureKeyVault:BaseUrl"];
                var secretName = "DbConnString";

                var client = new SecretClient(new Uri(keyVaultBaseUrl), new DefaultAzureCredential());
                var secret = client.GetSecret(secretName);

                return secret.Value.Value; 
            }
        }

        public static void InitializeDatabase(IConfiguration configuration)
        {

            var connectionString = GetDatabaseConnectionString(configuration);

            Console.WriteLine("Database connection string retrieved successfully.");

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString) 
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly(), 
                    script => script.EndsWith(".sql")) 
                .LogToConsole() 
                .Build(); 

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.WriteLine($"An error occurred while updating the database: {result.Error}");
                return;
            }

            Console.WriteLine("Database updated successfully.");
        }

    }
}
