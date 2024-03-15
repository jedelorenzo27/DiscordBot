using DiscordBot.services.azure_services;
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

        var secretsSource = Configuration["SecretsSource"];

        if (secretsSource == "AzureKeyVault")
        {
            var keyVaultBaseUrl = Configuration["AzureKeyVault:BaseUrl"];
            azureSecretsService = new AzureKeyVaultSecrets(keyVaultBaseUrl);
        }
    }

    private static async Task<string> GetSecretAsync(string key)
    {
        if (azureSecretsService != null)
        {
            // If AzureKeyVaultSecrets service is initialized, fetch secrets from Azure Key Vault
            return await azureSecretsService.GetSecretAsync(key);
        }
        else
        {
            // For "Local" SecretsSource, retrieve the value directly from configuration
            var localKey = Configuration[$"LocalSecrets:{key}"];
            return localKey;
        }
    }

    public static async Task<string> DiscordToken() => await GetSecretAsync("DiscordToken");

    public static async Task<string> OpenAiApiToken() => await GetSecretAsync("OpenAIKey");
}
