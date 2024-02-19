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
        public string name;
        public int health;
        public int weight;
        public int length;
        public string speciality; // optional? appears on the description bar below portrait
        public PokemonType type;
        public PokemonAttack[] moveset;
        public int retreatCost;
        public string flavorText;
        public HashSet<PokemonType> weaknesses;
        public string weaknessStrength;
        public HashSet<PokemonType> resistances;
        public string resistanceStrength;
        public string portraitFileName;
        public string illustrator;

        public string getHealthFormatted ()
        {
            return health + " HP";
        }
    }

    public class PokemonAttack
    {
        public string name;
        public string description;
        public int power;
        public PokemonType[] types;
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
}
