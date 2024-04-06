using Discord;
using Discord.Interactions;
using SpecterAI.pokemonStuff;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SpecterAI.services;
using System.Diagnostics;
using SpecterAI.Utilities;
using BotShared.Utilities;
using BotShared.models;
using BotShared;

namespace SpecterAI.commands
{
    public class PokemonCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Service { get; set; }

        [SlashCommand("pokemon2", "Does nothing")]
        public async Task PokemonHello(string name)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.Pokemon);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            await DeferAsync();

            PokemonGenerator generator = new PokemonGenerator();
            PokemonDefinition definition = await generator.generateAiPokemonDefinition(Context, name);

            PokemonRendererImageSharp renderer = new PokemonRendererImageSharp();
            await renderer.createTestPokemonCard(definition);

            FileAttachment renderedCard = new FileAttachment(Constants.RenderedPokemonCardsDirectory + definition.name + ".png");
            LinkedList<FileAttachment> list = new LinkedList<FileAttachment>();
            list.AddFirst(renderedCard);
            Action<MessageProperties> action = (x) => { x.Attachments = list; x.Content = "done"; };
            //await ModifyOriginalResponseAsync(action);
            await FollowupWithFilesAsync(list);
            stopwatch.Stop();
            //await FollowupAsync("Completed in " + stopwatch.ElapsedMilliseconds / 1000.0f + " seconds.");
        }
    }
}
