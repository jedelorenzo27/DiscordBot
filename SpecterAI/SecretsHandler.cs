using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SecretsHandler
{
    private static Dictionary<string, string> secrets;


    public static void LoadSecrets()
    {
        secrets = new Dictionary<string, string>();
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        String Root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        using (var streamReader = File.OpenText(Root + @"/resources/secrets.txt"))
        {
            var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string[] parts = line.Split(':');
                if (!line.StartsWith('#') && parts.Length == 2)
                {
                    Console.WriteLine("Adding secret entry for: " + parts[0]);
                    secrets.Add(parts[0], parts[1]);
                }
                else
                {
                    Console.WriteLine("Invalid Entry: \"" + line + "\"");
                }
            }
        }
    }

    private static string getSecret(string secret)
    {
        if (!secrets.ContainsKey(secret))
        {
            Console.WriteLine("Missing secret entry for: " + secret);
        }
        return secrets[secret];
    }


    public static string DiscordToken()
    {
        return getSecret("discord_token");
    }
}
