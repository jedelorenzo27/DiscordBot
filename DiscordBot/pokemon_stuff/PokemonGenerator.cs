using BotShared;
using Discord;
using Discord.Interactions;
using BotShared.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SpecterAI.pokemonStuff;
using SpecterAI.services;
using SpecterAI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI
{
    public class PokemonGenerator
    {
        public PokemonDefinition getTestDefinition()
        {
            PokemonDefinition pokemonDefinition = new PokemonDefinition();
            pokemonDefinition.health = 100;
            pokemonDefinition.name = "FireGuy";
            pokemonDefinition.length = 55;
            pokemonDefinition.weight = 12;
            pokemonDefinition.speciality = "Fire";
            pokemonDefinition.flavorText = "In the heartland of America, the stage is set for a classic baseball rivalry. This tale unfolds with two teams: the Midland Mavericks tradition,  they love.";
            pokemonDefinition.resistances = new HashSet<PokemonType> { PokemonType.FIRE };
            pokemonDefinition.resistanceStrength = "-10";
            pokemonDefinition.weaknesses = new HashSet<PokemonType> { PokemonType.WATER };
            pokemonDefinition.weaknessStrength = "+20";
            pokemonDefinition.type = PokemonType.FIRE;
            pokemonDefinition.portraitFileName = "testPortrait.png";
            pokemonDefinition.retreatCost = 4;
            pokemonDefinition.illustrator = "Jason Cole";
            pokemonDefinition.rarity = CardRarity.RARE;

            PokemonAttack attack1 = new PokemonAttack();
            attack1.attackType = PokemonType.FIRE;
            attack1.name = "Big attack move";
            attack1.description = "nothing of interest here";
            attack1.damage = "90";
            attack1.amount = 3;
            PokemonAttack attack2 = new PokemonAttack();
            attack2.attackType = PokemonType.GRASS;
            attack2.name = "Big attack move";
            attack2.description = "nothing of interest here";
            attack2.damage = "20+";
            attack2.amount = 4;
            PokemonAttack attack3 = attack1;
            pokemonDefinition.moveset = new List<PokemonAttack>() { attack1, attack2 };
            return pokemonDefinition;
        }


        private async Task updateStatus(SocketInteractionContext context, int step, int totalSteps, List<string> updateLogs, string message)
        {
            
            if (message.Length == 0 && updateLogs.Count > 0)
            {
                updateLogs[updateLogs.Count - 1] += "Done";
            } else
            {
                string formattedMessage = $"[{step}/{totalSteps}] {message}";
                updateLogs.Add(formattedMessage);
            }

            Action<MessageProperties> action = (x) => { x.Content = string.Join("\n", updateLogs); };
            await context.Interaction.ModifyOriginalResponseAsync(action);
        }

        private int getNumberInRange(Random random, int min, int max)
        {
            int difference = max - min;
            return random.Next(difference) + min;
        }

        private int[] ExtractNumbersFromString(string message)
        {
            for(int i = 0; i < message.Length; i++)
            {
                if (message[i] != ' ' && !Char.IsLetter(message[i]) && !Char.IsDigit(message[i]))
                {
                    message.Replace(message[i], ' ');
                }
            }
            List<int> numbers = new List<int>();

            string[] words = message.Split(' ');
            foreach(string word in words)
            {
                int number = -1;
                if (int.TryParse(word, out number))
                {
                    numbers.Add(number);
                }   
            }
            return numbers.ToArray();
        }

        private PokemonType? ExtractMostReferencedPokemonType(string message, HashSet<PokemonType> excludedTypes)
        {
            int maxVisits = 0;
            string mostSeenType = "";

            PokemonType[] allTypes = Enum.GetValues(typeof(PokemonType)).Cast<PokemonType>().ToArray();

            List<string> approvedTypes = new List<string>();
            foreach(PokemonType type in allTypes)
            {
                if (!excludedTypes.Contains(type))
                {
                    approvedTypes.Add(Enum.GetName(type).ToLower());    
                }
            }

            foreach(string pokemonType in approvedTypes)
            {
                int count = CountPokemonTypeInMessage(pokemonType, message);
                if (count > maxVisits)
                {
                    maxVisits = count;
                    mostSeenType = pokemonType;
                }
            }

            PokemonType mostSeen;
            if (Enum.TryParse(mostSeenType.ToUpper(), out mostSeen))
            {
                return mostSeen;
            }
            return null;

        }
        private int CountPokemonTypeInMessage(string pokemonType, string message)
        {
            int count = 0;
            foreach (string word in message.ToLower().Split(' '))
            {
                if (word.Contains(pokemonType))
                {
                    count++;
                }
            }
            return count;
        }


        public async Task<PokemonDefinition> generateAiPokemonDefinition(SocketInteractionContext context, string name)
        {
            List<string> updateLogs = new List<string>();
            int totalSteps = 12;
            int generationStep = 0;
            await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Gathering definition information...");
            Random random = new Random();
            PokemonDefinition definition = new PokemonDefinition();
            definition.name = name;
            Conversation conversation = new Conversation();

            try
            {
                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Imagining Pokemon's description...");
                await GetPokemonDescription(context, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's type...");
                await GetPokemonType(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's weaknesses...");
                await GetPokemonWeakness(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's resistances...");
                await GetPokemonResistance(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's weight...");
                await GetPokemonWeight(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's height...");
                await GetPokemonHeight(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's attacks...");
                await GetPokemonAttacks(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon's retreat cost...");
                await GetRetreatCost(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Writing flavor text...");
                await GetFlavorText(context, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Determining Pokemon rarity...");
                await GetRarity(context, random, definition, conversation);
                await GetRarity(context, random, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                await updateStatus(context, ++generationStep, totalSteps, updateLogs, "Snapping a sic pic of your rad new 'mon...");
                await GetPortrait(context, definition, conversation);
                await updateStatus(context, generationStep, totalSteps, updateLogs, "");

                definition.illustrator = "Jason Cole";
            }
            catch (Exception e)
            {
                await Logger.LogMessage(LogLevel.Error, $"Something broke af, error: {e.Message}");
                await Logger.LogMessage(LogLevel.Error, $"{e.StackTrace}");

                await updateStatus(context, generationStep, totalSteps, updateLogs, "Sorry, you're Pokemon is gone and probably dead.");
            }

            await updateStatus(context, generationStep, totalSteps, new List<string>(), definition.ToString());
            return definition;
        }

        private async Task GetPokemonDescription(SocketInteractionContext context, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, $"Imagine we're creating a new pokemon named {definition.name}.");
            conversation.addMessage(MessageRole.USER, "Describe what this new pokemon would look like.");
            string description = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, description);
            definition.description = description;
        }
        
        private async Task GetPokemonType(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, $"What type best describes this pokemon?");
            conversation.addMessage(MessageRole.USER, $"This pokemon can be any of the following types: {string.Join(", ", Enum.GetNames(typeof(PokemonType)))}");
            string pokemonType = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, pokemonType);

            PokemonType? possibleType = ExtractMostReferencedPokemonType(pokemonType, new HashSet<PokemonType>());
            if (possibleType != null)
            {
                definition.type = (PokemonType)possibleType;
            } else
            {
                await Logger.LogMessage(LogLevel.Error, $"Failed to extract pokemon type from: {pokemonType}");
                conversation.addMessage(MessageRole.USER, $"This pokemon is Normal Type.");
                definition.type = PokemonType.NORMAL;
            }
        }

        private async Task GetPokemonWeakness(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, $"Is this pokemon weak to any of the following types: {string.Join(", ", Enum.GetNames(typeof(PokemonType)))}?");
            string pokemonWeaknessResponse = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, pokemonWeaknessResponse);

            PokemonType? possibleType = ExtractMostReferencedPokemonType(pokemonWeaknessResponse, new HashSet<PokemonType>() { definition.type});
            if (possibleType != null)
            {
                definition.weaknesses = new HashSet<PokemonType>
                {
                    (PokemonType)possibleType
                };
                definition.weaknessStrength = PokemonDefinition.WeaknessStrengths[random.Next(PokemonDefinition.WeaknessStrengths.Length - 1)];
            }
            else
            {
                await Logger.LogMessage(LogLevel.Error, $"Failed to extract weakness from: {pokemonWeaknessResponse}");
                //Do not give a default weakness
            }
        }

        private async Task GetPokemonResistance(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, $"is this pokemon resistant to any of the following types: {string.Join(", ", Enum.GetNames(typeof(PokemonType)))}?");
            string pokemonResistanceResponse = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, pokemonResistanceResponse);
            PokemonType? possibleType = ExtractMostReferencedPokemonType(pokemonResistanceResponse, new HashSet<PokemonType>(definition.weaknesses));
            if (possibleType != null)
            {
                definition.resistances = new HashSet<PokemonType>() ;
                definition.resistances.Add((PokemonType)possibleType);
                definition.resistanceStrength = PokemonDefinition.ResistanceStrengths[random.Next(PokemonDefinition.ResistanceStrengths.Length - 1)];

            }
            else
            {
                await Logger.LogMessage(LogLevel.Error, $"Failed to extract weakness from: {pokemonResistanceResponse}");
                //Do not give a default resistance
            }
        }

        private async Task GetPokemonWeight(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, "How much does this Pokemon weigh in pounds?");
            string weight = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, weight);
            int[] responseNumbers = ExtractNumbersFromString(weight);
            if (responseNumbers.Length == 1)
            {
                definition.minWeight = responseNumbers[0];
                definition.maxWeight = responseNumbers[0];
                definition.weight = responseNumbers[0];
            }
            else if (responseNumbers.Length > 1)
            {
                Array.Sort(responseNumbers);
                definition.minWeight = responseNumbers[0];
                definition.maxWeight = responseNumbers[responseNumbers.Length - 1];
                definition.weight = getNumberInRange(random, responseNumbers[0], responseNumbers[responseNumbers.Length - 1]);
            }
            // else, weight simply won't be rendered on card
        }

        private async Task GetPokemonHeight(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            definition.health = random.Next(95) + 5;
        }

        private async Task GetPokemonAttacks(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            definition.moveset = new List<PokemonAttack>();
            definition.moveset.Add(await (GetPokemonAttack(context, random,  definition, conversation)));
            if (random.Next(10) > 3)
            {
                definition.moveset.Add(await (GetPokemonAttack(context, random, definition, conversation)));
            }
        }

        private async Task<PokemonAttack> GetPokemonAttack(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            PokemonAttack attack = new PokemonAttack();

            conversation.addMessage(MessageRole.USER, "What is the name of an attack move this Pokemon might have. Only provide the name. Do not describe the attack move in any way.");
            string attackName = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, attackName);
            attack.name = attackName;

            conversation.addMessage(MessageRole.USER, $"Very briefly describe {attackName}");
            string attackDescription = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, attackName);
            attack.description = attackDescription;

            conversation.addMessage(MessageRole.USER, $"From the following types: {string.Join(", ", Enum.GetNames(typeof(PokemonType)))}, what type of attack is {attackName}? ");
            string attackTypeResponse = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, attackTypeResponse);

            PokemonType? attackType = ExtractMostReferencedPokemonType(attackTypeResponse, new HashSet<PokemonType>());
            if (attackType != null)
            {
                attack.attackType = (PokemonType)attackType;
            }
            
            attack.amount = random.Next(3) + 1;
            attack.damage = ((random.Next(9) + 1) * 10).ToString();

            return attack;
        }

        private async Task GetRetreatCost(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, "What should the retreat cost for this pokemon be?");
            conversation.addMessage(MessageRole.USER, "Retreat cost must be between 0 and 4");
            string restreatCostResponse = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, restreatCostResponse);
            int[] possibleRetreatCosts = ExtractNumbersFromString(restreatCostResponse);
            if (possibleRetreatCosts.Length == 0)
            {
                definition.retreatCost = 0;
            } else
            {
                definition.retreatCost = possibleRetreatCosts[random.Next(possibleRetreatCosts.Length - 1)];
            }
        }

        private async Task GetRarity(SocketInteractionContext context, Random random, PokemonDefinition definition, Conversation conversation)
        {
            List<CardRarity> rarityList = new List<CardRarity>();
            if (random.Next(100) > 95)
            {
                definition.rarity = CardRarity.RARE;
            } else if (random.Next(100) > 75)
            {
                definition.rarity = CardRarity.UNCOMMON;
            } else
            {
                definition.rarity = CardRarity.COMMON;
            }
        }

        private async Task GetFlavorText(SocketInteractionContext context, PokemonDefinition definition, Conversation conversation)
        {
            conversation.addMessage(MessageRole.USER, "Write some very short flavor text in the style you might find on a pokemon card for this creature.");
            string flavorText = await OpenAIServices.Chat(conversation);
            conversation.addMessage(MessageRole.SYSTEM, flavorText);
            definition.flavorText = flavorText;
            //TODO: truncate long flavor text
        }

        private async Task GetPortrait(SocketInteractionContext context, PokemonDefinition definition, Conversation conversation)
        {
            string imagePrompt = $"Imagine a newly discovered pokemon named {definition.name}. It is described as follows: '{definition.description}' in it's natural habitat. Add a nice nature scene to background. DO NOT ADD TEXT!";
            imagePrompt += $"This pokemon weighs {definition.weight}. ";
            imagePrompt += $"This pokemon is {definition.type} type. ";
            definition.portraitFileName = $"{definition.name}{Constants.PokemonPortraitFileSuffix}.png";
            await OpenAIServices.Image(imagePrompt, Constants.PokemonPortraitsDirectory, definition.portraitFileName);
            
        }

    }
}
