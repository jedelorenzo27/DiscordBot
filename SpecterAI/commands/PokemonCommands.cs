using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.commands
{
    public class PokemonCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("pokemon2", "Does nothing")]
        public async Task PokemonHello(string name)
        {
            Console.WriteLine("Returning pokemon card");
            FileAttachment renderedCard = new FileAttachment(Utilities.getRenderedPokemonCardsDirectory() + "Bumper.png");
            Console.WriteLine(renderedCard.FileName);
            await RespondWithFileAsync(renderedCard);
        }
    }
}
