using SpecterAI.Utilities;
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
        public static string[] ResistanceStrengths
        {
            get
            {
                return new string[]
                {
                    "-10",
                    "-20",
                    "-30",
                    "-40"

                };
            }
        }

        public static string[] WeaknessStrengths
        {
            get
            {
                return new string[]
                {
                    "+10",
                    "+20",
                    "+30",
                    "+40",
                    "x2"
                };
            }
        }


/// <summary>
/// Used to render a portrait
/// </summary>
public string description;
        
        
        public string name = "<missing>";
        public int health = 0;
        public int weight = 0;
        public int minWeight = 0; //optional
        public int maxWeight = 0; //optional
        public int length = 0;
        public string speciality = ""; // optional? appears on the description bar below portrait
        public PokemonType type = PokemonType.NORMAL;
        public List<PokemonAttack> moveset = new List<PokemonAttack>();
        public int retreatCost = 0;
        public string flavorText = "";
        public HashSet<PokemonType> weaknesses = new HashSet<PokemonType>();
        public string weaknessStrength = "0";
        public HashSet<PokemonType> resistances = new HashSet<PokemonType>();
        public string resistanceStrength = "";
        public string portraitFileName = "";
        public string illustrator = "";
        public CardRarity rarity = CardRarity.COMMON;

        public string getHealthFormatted ()
        {
            return health + " HP";
        }




        override
        public string ToString()
        {
            List<string> list = new List<string>();
            list.Add($"name:                {name}");
            list.Add($"health:              {health}");
            list.Add($"weight:              {weight}");
            list.Add($"minWeight:           {minWeight}");
            list.Add($"maxWeight:           {maxWeight}");
            list.Add($"length:              {length}");
            list.Add($"speciality:          {speciality}");
            list.Add($"type:                {Enum.GetName(type)}");
            list.Add($"retreatCost:         {retreatCost}");
            list.Add($"flavorText:          {flavorText}");
            list.Add($"weaknesses:          {string.Join(',', weaknesses)}");
            list.Add($"weaknessStrength:    {weaknessStrength}");
            list.Add($"resistances:         {string.Join(',', resistances)}");
            list.Add($"resistanceStrength:  {resistanceStrength}");
            list.Add($"portraitFileName:    {portraitFileName}");
            list.Add($"illustrator:         {illustrator}");
            list.Add($"rarity:              {Enum.GetName(rarity)}");
            return $"PokemonDefinition [\n\t{string.Join("\n\t", list.ToArray())} \n]";
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
