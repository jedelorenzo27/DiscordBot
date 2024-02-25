using Discord;
using Discord.Interactions;
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
        //https://github.com/OkGoDoIt/OpenAI-API-dotnet
        //private static OpenAIAPI? api;
        //private static Conversation conversation;

        public async static Task<string> Chat(HttpClient client, Conversation conversation)
        {

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions"))
            {

                var requestData = new
                {
                    messages = conversation.conversationArray,
                    temperature = 0.7,
                    max_tokens = 60,
                    model = "gpt-4-0125-preview"
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SecretsHandler.OpenAIApiToken());
                requestMessage.Content = contentString;
                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                Console.Write(responseMessage.Content);
                string stringContent = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    conversation.printConversation();
                    dynamic something = JsonConvert.DeserializeObject<dynamic>(stringContent);
                    return something.choices[0].message.content;
                } catch (Exception ex)
                {
                    return "Something went wrong, likely while pulling data from chatGPTs response.\nstringContent: " + stringContent + "\nException: " + ex.Message;
                }
            }
        }

        public static async Task<string> Image(HttpClient client, string prompt)
        {

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations"))
            {
                Console.WriteLine("Get image from OpenAI");
                var requestData = new
                {
                    model = "dall-e-3",
                    prompt = prompt,
                    num_images = 1,
                    size = "1024x1024"
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SecretsHandler.OpenAIApiToken());
                requestMessage.Content = contentString;
                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                Console.WriteLine(responseMessage);
                string stringContent = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    dynamic something = JsonConvert.DeserializeObject<dynamic>(stringContent);
                    string imageURL = something.data[0].url;
                    Console.WriteLine(stringContent);

                    var res = await client.GetAsync(imageURL);

                    byte[] bytes = await res.Content.ReadAsByteArrayAsync();
                    string tempImageName = "tempimage.png";
                    await HttpUtilities.DownloadFileAsync(client, imageURL, GeneralUtilities.outputDirectory + @"temp\" + tempImageName);
                    return tempImageName;
                }
                catch (Exception ex)
                {
                    return "Something broke: " + ex.Message;
                    //return "Something went wrong, likely while pulling data from chatGPTs response.\nstringContent: " + stringContent + "\nException: " + ex.Message;
                }
            }
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
