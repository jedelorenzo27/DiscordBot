using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.Utilities
{
    public static class GeneralUtilities
    {


        public static string resourceDirectory
        {
            get
            {
                return getRootDirectory() + Path.DirectorySeparatorChar + "resources" + Path.DirectorySeparatorChar;
            }
        }

        public static string outputDirectory
        {
            get
            {
                return getRootDirectory() + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;
            }
        }

        public static string pokemonPortraitsDirectory
        {
            get
            {
                return outputDirectory + "pokemon_portraits" + Path.DirectorySeparatorChar;
            }
        }

        public static string renderedPokemonCardsDirectory
        {
            get
            {
                return outputDirectory + "pokemon_cards" + Path.DirectorySeparatorChar;
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
