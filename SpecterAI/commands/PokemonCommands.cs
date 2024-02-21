using Discord;
using Discord.Interactions;
using SpecterAI.pokemonStuff;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpecterAI.services;

namespace SpecterAI.commands
{
    public class PokemonCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Service { get; set; }

        [SlashCommand("pokemon2", "Does nothing")]
        public async Task PokemonHello(string name)
        {

            string response = await OpenAIServices.Chat(name);
            await RespondAsync(response);
            return;



            await DeferAsync();
            Thread.Sleep(1000);
            for(int i = 0; i < 5; i++)
            {
                Thread.Sleep(1000);
                Action<MessageProperties> action2 = (x) => x.Content = "Second: " + i;
                await ModifyOriginalResponseAsync(action2);
            }

            
            PokemonRendererImageSharp renderer = new PokemonRendererImageSharp();
            renderer.createTestPokemonCard();
            FileAttachment renderedCard = new FileAttachment(Utilities.getRenderedPokemonCardsDirectory() + "FireGuy.png");

            LinkedList<FileAttachment> list = new LinkedList<FileAttachment>();
            list.AddFirst(renderedCard);

            Action<MessageProperties> action = (x) => { x.Attachments = list; x.Content = "done"; };
            await ModifyOriginalResponseAsync(action);

            //Console.WriteLine("Returning pokemon card");
            //Console.WriteLine(renderedCard.FileName);
            //await RespondWithFileAsync(renderedCard);
        }
    }
}
