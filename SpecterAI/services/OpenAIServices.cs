using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.services
{
    public static class OpenAIServices
    {
        //https://github.com/OkGoDoIt/OpenAI-API-dotnet
        private static OpenAIAPI? api;

        public static void InitOpenAi()
        {
            api = new OpenAIAPI(SecretsHandler.OpenAIApiToken());
        }

        public async static Task<string> Chat(string prompt)
        {
            var result = await api.Chat.CreateChatCompletionAsync(prompt);
            Console.WriteLine(result);
            return result.ToString();
        }
    }
}
