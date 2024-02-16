using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI
{
    public static class Utilities
    {

        public static string getResourceDirectory()
        {
            return getRootDirectory() + @"\resources\";
        }

        public static string getOutputDirectory ()
        {
            return getRootDirectory() + @"\output\";
        }

        public static string getPokemonPortraitsDirectory()
        {
            return getOutputDirectory() + @"\pokemon_portraits\";
        }

        public static string getRenderedPokemonCardsDirectory()
        {
            return getOutputDirectory() + @"\pokemon_cards\";
        }

        private static string getRootDirectory()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String Root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return Root;
        }

    }
}
