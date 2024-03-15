using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.services.azure_services
{
    internal class AzureKeyValutKeys
    {
        private readonly KeyClient _keyClient;

        public AzureKeyValutKeys(string keyVaultUrl)
        {
            _keyClient = new KeyClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public async Task<string> GetKeyAsync(string keyName)
        {
            KeyVaultKey key = await _keyClient.GetKeyAsync(keyName);
            return key.ToString();
        }
    }
}
