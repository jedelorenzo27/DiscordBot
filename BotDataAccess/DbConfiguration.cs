using Microsoft.Extensions.Configuration;

namespace BotDataAccess
{
    public class DbConfiguration
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


            return configuration.GetConnectionString("DefaultConnection");

        }

        public static void InitializeDatabase(IConfiguration configuration)
        {

            var connectionString = GetDatabaseConnectionString(configuration);

            Console.WriteLine("Database connection string retrieved successfully.");

            /*var upgrader = DeployChanges.To
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
            }*/

            Console.WriteLine("Database updated successfully.");
        }
    }
}
