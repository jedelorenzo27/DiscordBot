using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SpecterAI.pokemonStuff;
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

       



    }
}
