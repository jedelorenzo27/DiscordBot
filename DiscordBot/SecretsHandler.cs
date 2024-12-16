using BotShared.services.azure_services;
using Microsoft.Extensions.Configuration;

public static class SecretsHandler
{
    private static IConfigurationRoot Configuration;
    private static AzureKeyVaultSecrets azureSecretsService;

    public static void InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    private static async Task<string> GetSecretAsync(string key)
    {
        var localKey = Configuration[$"Secrets:{key}"];
        return localKey;
    }

    public static async Task<string> DiscordToken() => await GetSecretAsync("DiscordToken");

    public static async Task<string> OpenAiApiToken() => await GetSecretAsync("OpenAIKey");
}
