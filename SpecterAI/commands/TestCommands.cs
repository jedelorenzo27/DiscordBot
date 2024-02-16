using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.commands
{
    public class TestCommands : ModuleBase<SocketCommandContext>
    {
        [Command("beans")]
        [Summary("Echoes the input back to you.")]
        public async Task EchoAsync([Remainder] string input)
        {
            Console.WriteLine($"Echo {input}");
            await ReplyAsync(input);
        }
    }
}
