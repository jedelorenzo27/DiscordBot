using BotShared.models;
using Discord;
using Discord.Interactions;
using DiscordBot.Utilities;
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
            await PermissionsService.ValidatePermissions(Context, Entitlement.OpenAiChat);

            Stopwatch stopwatch = new Stopwatch();
            await DeferAsync();
            conversation.addMessage(MessageRole.USER, prompt);
            string response = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, response);
            Action<MessageProperties> action = (x) => { x.Content = response; };
            await ModifyOriginalResponseAsync(action);
            stopwatch.Stop();
        }

        [SlashCommand("clear-chat", "Clears open AI's chat conversation")]
        public async Task ClearChatConversation()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.OpenAiChat);
            conversation = new Conversation();
            await RespondAsync("Conversation cleared");
        }


        [SlashCommand("image", "Render an image using Open Ai's chatGPT")]
        public async Task OpenAiImage(string prompt)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.OpenAiImage);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await DeferAsync();
            Action<MessageProperties>? action = (x) => { x.Content = "Generating image..."; };
            await ModifyOriginalResponseAsync(action);

            string response = await OpenAIServices.Image(prompt);
            string fileLocation = Constants.TempDirectory + response;

            if (File.Exists(fileLocation) || true)
            {
                action = (x) => { x.Content = "Uploading image to discord..."; };
                await ModifyOriginalResponseAsync(action);
                FileAttachment image = new FileAttachment(fileLocation);
                List<FileAttachment> list = new List<FileAttachment>();
                list.Add(image);
                stopwatch.Stop();
                action = (x) => { x.Attachments = list; x.Content = "Completed in " + stopwatch.ElapsedMilliseconds / 1000.0f + " seconds"; };
                await ModifyOriginalResponseAsync(action);
                image.Dispose();
            } else
            {
                action = (x) => { x.Content = "Something went wrong with: " + response + @". Can't find location: " + fileLocation; };
                await ModifyOriginalResponseAsync(action);
            }
            
        }

    }
}
