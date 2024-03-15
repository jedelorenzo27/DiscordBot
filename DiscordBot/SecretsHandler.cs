using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SecretsHandler
{
    private static Dictionary<string, string> secrets;


    public static async Task LoadSecrets()
    {
        secrets = new Dictionary<string, string>();
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        String Root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        string secretsFile = Root + Path.DirectorySeparatorChar + "resources" + Path.DirectorySeparatorChar + "secrets.txt";
        if (!File.Exists(secretsFile))
        {
            string[] errors = new string[]
            {
                "Can't find secrets file. You're either missing it entirely or have it in the wrong location.",
                "secrets.txt needs to be under the resources folder 'resources/secrets.txt"
            };
            await LoggingService.LogMessage(LogLevel.ConfigError, errors);
            return;
        }


        using (var streamReader = File.OpenText(secretsFile))
        {
            var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string[] parts = line.Split(':');
                if (!line.StartsWith('#') && parts.Length == 2)
                {
                    await LoggingService.LogMessage(LogLevel.ConfigError, $"Adding secret entry for: {parts[0]}");
                    Console.WriteLine("Adding secret entry for: " + parts[0]);
                    secrets.Add(parts[0], parts[1]);
                }
            }
        }
    }

    private static string getSecret(string secret)
    {
        if (!secrets.ContainsKey(secret))
        {
            Console.WriteLine("Missing secret entry for: " + secret);
            return " ";
        }
        return secrets[secret];
    }

    public static string DiscordToken
    {
        get
        {
            return getSecret("discord_token");
        }
    }

    public static string OpenAiApiToken
    {
        get
        {
            return getSecret("openai_token");
        }
    }
}
