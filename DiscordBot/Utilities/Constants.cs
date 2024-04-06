using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utilities
{
    public static class Constants
    {
        public static Version BotVersion = new Version(1,0,1);


        public static ulong ByteLordzCringeCave_Role_Unemployed = 1211510658282164315;
        public static ulong ByteLordzCringeCave_Role_Everyone = 1211508294179356734;
        public static ulong ByteLordzCringeCave_Channel_The_Daily = 1211510141191593984;
        public static ulong ByteLordzCringeCave_Channel_Tomorrows_Daily = 1212274566030954556;

        public static ulong ByteLordzCringeCave_Challenge_Log_Message = 1217141601109938297;

        // Directory/File Constants
        private static string getRootDirectory()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String Root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return Root;
        }
        public static char slash = Path.DirectorySeparatorChar;

        public static string ResourceDirectory = $"{getRootDirectory()}{slash}resources{slash}";
        public static string OutputDirectory = $"{getRootDirectory()}{slash}output{slash}";
        public static string TempDirectory = $"{OutputDirectory}temp{slash}";

        public static string DevLogsChannelId = "1103512627675672608";
        public static string JayUserId = "222127402980081667";
        public static string JonathanUserId = "429310221861519374";
        public static string ChrisUserId = "447113923162800148";        
        
        // Pokemon
        public static string FontDirectory = $"{ResourceDirectory}fonts{slash}";
        public static string PokemonPortraitsDirectory = $"{OutputDirectory}pokemon_portraits{slash}";
        public static string RenderedPokemonCardsDirectory = $"{OutputDirectory}pokemon_cards{slash}";
    }
}
