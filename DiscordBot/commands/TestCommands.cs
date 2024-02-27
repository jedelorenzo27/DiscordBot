using Discord.Commands;
using Discord.Interactions;
using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.commands
{
    public class TestCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("test", "Implement with whatever neeeds testing")]
        public async Task testCommand(string input1, string input2)
        {
            await LoggingService.LogMessage(LogLevel.Debug, input1);
            await RespondAsync("For testing new slash commands while waiting for the UI to update - Currently does nothing");
        }
    }
}
