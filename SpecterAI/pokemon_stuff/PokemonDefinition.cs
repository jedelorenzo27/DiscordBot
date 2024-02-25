using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.pokemonStuff
{
    //https://card-pokemon.com/
    public class PokemonDefinition
    {
        /// <summary>
        /// Used to render a portrait
        /// </summary>
        public string description;
        
        
        public string name;
        public int health;
        public int weight;
        public int length;
        public string speciality; // optional? appears on the description bar below portrait
        public PokemonType type;
        public List<PokemonAttack> moveset;
        public int retreatCost;
        public string flavorText;
        public HashSet<PokemonType> weaknesses;
        public string weaknessStrength;
        public HashSet<PokemonType> resistances;
        public string resistanceStrength;
        public string portraitFileName;
        public string illustrator;
        public CardRarity rarity = CardRarity.COMMON;

        public string getHealthFormatted ()
        {
            return health + " HP";
        }
    }

    public class PokemonAttack
    {
        public string name;
        public string description;
        public string damage;
        public int amount;
        public PokemonType attackType;
    }

    public enum PokemonType
    {
        DARK,
        ELECTRIC,
        FIGHTING,
        FIRE,
        GRASS,
        NORMAL,
        PSYCHIC,
        STEEL,
        WATER
    }

    public enum CardRarity
    {
        COMMON,
        UNCOMMON,
        RARE
    }

}
