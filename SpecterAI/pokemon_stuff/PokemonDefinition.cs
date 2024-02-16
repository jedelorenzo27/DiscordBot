using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.pokemonStuff
{
    public class PokemonDefinition
    {
        public string name;
        public int health;
        public string description;
        public PokemonType type;
        public PokemonAttack[] moveset;
        public int retreatCost;
        public string flavorText;
        public HashSet<PokemonType> weaknesses;
        public HashSet<PokemonType> resistances;
        public string portraitFileName;

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
