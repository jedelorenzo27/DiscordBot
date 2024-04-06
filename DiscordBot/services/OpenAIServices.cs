using Discord;
using Discord.Interactions;
using BotShared.Utilities;
using Newtonsoft.Json;
using SpecterAI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BotShared;

namespace SpecterAI.services
{

    public class Conversation 
    {
        private List<Message> messages = new List<Message>();

        public void addMessage(MessageRole role, string content)
        {
            addMessage(new Message(role, content));
        }

        public void addMessage(Message message)
        {
            messages.Add(message);
        }

        public Dictionary<string,string>[] conversationArray
        {
            get
            {
                Dictionary<string, string>[] conversation = new Dictionary<string, string>[messages.Count];
                for (int i = 0; i < messages.Count; i++)
                {
                    conversation[i] = messages[i].message;
                }
                return conversation;
            }
        }

        public void printConversation()
        {
            foreach (Message message in messages)
            {
                Console.WriteLine(message.role + ": " + message.content);
            }
        }

    }

    public enum MessageRole
    {
        [Description("user")]
        USER,
        [Description("system")]
        SYSTEM,
        [Description("assistant")]
        ASSISTANT
    }

    public class Message {
        
        public MessageRole role { get; private set; }
        public string content { get; private set; }

        public Message(MessageRole role, string content)
        {
            this.role = role;
            this.content = content;
        }

        public Dictionary<string, string> message
        {
            get {
                Dictionary<string, string> messageDictionary = new Dictionary<string, string>();
                messageDictionary.Add("role", EnumExtensions.ToDescriptionString(role));
                messageDictionary.Add("content", content);
                return messageDictionary;
            }
        }
    }

    public static class OpenAIServices
    {
        public async static Task<string> Chat(Conversation conversation)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions"))
            {
                var requestData = new
                {
                    messages = conversation.conversationArray,
                    temperature = 0.7,
                    max_tokens = 300,
                    model = "gpt-4-0125-preview"
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await SecretsHandler.OpenAiApiToken());
                requestMessage.Content = contentString;
                HttpResponseMessage responseMessage = await Program._httpClient.SendAsync(requestMessage);
                string stringContent = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    //conversation.printConversation();
                    dynamic something = JsonConvert.DeserializeObject<dynamic>(stringContent);
                    return something.choices[0].message.content;
                } catch (Exception ex)
                {
                    return "Something went wrong, likely while pulling data from chatGPTs response.\nstringContent: " + stringContent + "\nException: " + ex.Message;
                }
            }
        }

        public static async Task<string> Image(string prompt, string saveDirectory, string fileName)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations"))
            {
                var requestData = new
                {
                    model = "dall-e-3",
                    prompt = prompt,
                    num_images = 1,
                    size = "1024x1024"
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await SecretsHandler.OpenAiApiToken());
                requestMessage.Content = contentString;
                HttpResponseMessage responseMessage = await Program._httpClient.SendAsync(requestMessage);
                string stringContent = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    dynamic something = JsonConvert.DeserializeObject<dynamic>(stringContent);
                    string imageURL = something.data[0].url;
                    var res = await Program._httpClient.GetAsync(imageURL);

                    byte[] bytes = await res.Content.ReadAsByteArrayAsync();
                    Console.WriteLine($"imageURL: {imageURL}");
                    Console.WriteLine($"saveDirectory: {saveDirectory}");
                    Console.WriteLine($"fileName: {fileName}");
                    await HttpUtilities.DownloadFileAsync(Program._httpClient, imageURL, saveDirectory + fileName);
                    return fileName;
                }
                catch (Exception ex)
                {
                    return "Something broke: " + ex.Message;
                }
            }
        }

        public static async Task<string> Image(string prompt)
        {
            string tempImageName = "tempimage.png";
            return await Image(prompt, Constants.OutputDirectory + @"temp" + Path.DirectorySeparatorChar, tempImageName);
        }

        

        public async static Task<string> ChatBroken(SocketInteractionContext openAiContext, string prompt)
        {

            return null;
            /*
            var result = await api.Chat.CreateChatCompletionAsync(prompt);
            ChatMessage message = new ChatMessage();
            message.TextContent = prompt;
            conversation.AppendMessage(message);
            conversation.AppendUserInput(prompt);
            Console.WriteLine(result);
            return result.ToString();
            */

            /*
            Action<MessageProperties> action = (x) => { };
            await context.Interaction.ModifyOriginalResponseAsync(action, null)
                Action<MessageProperties> action = (x) => { x.Content = response; };

            var completionRequest = new CompletionRequest
            {
                Prompt = "Once upon a time, in a land far, far away...",
                MaxTokens = 150, // Increase this value for longer responses
                Temperature = 0.7,
                TopP = 1,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };
            */





            //await api.Chat.StreamChatEnumerableAsync(res =>
            //{
            //    Console.Write(res);
            //});


            /*
            ChatRequest chatRequest = new ChatRequest();
            chatRequest.MaxTokens = 300;
            chatRequest.Temperature = 0.9f;
            chatRequest.Model = Model.GPT4_32k_Context;
            chatRequest
            */
            //var result = await api.Chat.CreateChatCompletionAsync(chatRequest);
            // Output the result
            //Console.WriteLine(result);
            //return result.ToString();

        }
    }
}
