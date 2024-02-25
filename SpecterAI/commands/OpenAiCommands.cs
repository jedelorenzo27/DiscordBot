using Discord;
using Discord.Interactions;
using SpecterAI.services;
using SpecterAI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.commands
{
    public class OpenAiCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private static Conversation conversation = new Conversation();

        [SlashCommand("chat", "Chat with Open AI")]
        public async Task OpenAiChat(string prompt)
        {
            Stopwatch stopwatch = new Stopwatch();
            await DeferAsync();
            conversation.addMessage(MessageRole.USER, prompt);
            string response = await OpenAIServices.Chat(Program._httpClient, conversation);
            conversation.addMessage(MessageRole.SYSTEM, response);
            Action<MessageProperties> action = (x) => { x.Content = response; };
            await ModifyOriginalResponseAsync(action);
            stopwatch.Stop();
        }

        [SlashCommand("clear-chat", "Clears open AI's chat conversation")]
        public async Task ClearChatConversation()
        {
            conversation = new Conversation();
            await RespondAsync("Conversation cleared");
        }


        [SlashCommand("image", "Render an image using Open Ai's chatGPT")]
        public async Task OpenAiImage(string prompt)
        {
            Stopwatch stopwatch = new Stopwatch();
            await DeferAsync();
            Action<MessageProperties> action = (x) => { x.Content = "Generating image..."; };
            await ModifyOriginalResponseAsync(action);

            string response = await OpenAIServices.Image(Program._httpClient, prompt);
            string fileLocation = GeneralUtilities.outputDirectory + @"temp\" + response;

            if (File.Exists(fileLocation) || true)
            {
                action = (x) => { x.Content = "Uploading image to discord..."; };
                await ModifyOriginalResponseAsync(action);


                FileAttachment image = new FileAttachment(fileLocation);
                LinkedList<FileAttachment> list = new LinkedList<FileAttachment>();
                list.AddFirst(image);
                action = (x) => { x.Attachments = list; x.Content = "..."; };
                await ModifyOriginalResponseAsync(action);
            } else
            {
                action = (x) => { x.Content = "Something went wrong with: " + response + @". Can't find location: " + fileLocation; };
                await ModifyOriginalResponseAsync(action);
            }
            stopwatch.Stop();
        }

    }
}
