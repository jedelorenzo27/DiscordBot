using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.Utilities
{
    public static class GeneralUtilities
    {


        public static string resourceDirector
        {
            get
            {
                return getRootDirectory() + @"\resources\";
            }
        }

        public static string outputDirectory
        {
            get
            {
                return getRootDirectory() + @"\output\";
            }
        }

        public static string pokemonPortraitsDirectory
        {
            get
            {
                return getRootDirectory() + @"\pokemon_portraits\";
            }
        }

        public static string renderedPokemonCardsDirectory
        {
            get
            {
                return getRootDirectory() + @"\pokemon_cards\";
            }
        }

        private static string getRootDirectory()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String Root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return Root;
        }

    }
}
