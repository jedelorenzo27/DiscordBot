using Discord;
using Discord.Interactions;
using DiscordBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.commands
{
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("admin-version", "Returns the current running version")]
        public async Task GetVersion()
        {
            await DeferAsync();
            Action<MessageProperties> action = (x) => { x.Content = $"Current version: {Constants.BotVersion}"; };
            await ModifyOriginalResponseAsync(action);
        }
    }
}
