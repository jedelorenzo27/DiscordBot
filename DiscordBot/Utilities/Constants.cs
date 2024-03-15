using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utilities
{
    public static class Constants
    {
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

        // LoggingService:
        public static string DebugChannelsFile = $"{ResourceDirectory}logging{slash}debug_channels.txt";

        // Permission:
        //public static 
        public static string BasePermissionPath = $"{ResourceDirectory}permissions{slash}";
        public static string PermissionsFileName = $"permissions.txt";
        public static string PermissionsFilePath = $"{BasePermissionPath}{PermissionsFileName}";
        public static string BannedUsersFileName = $"banned_users.txt";
        public static string BannedUsersFilePath = $"{BasePermissionPath}{BannedUsersFilePath}";
        public static string UserMetadataFileName = $"user_metadata.txt";
        public static string UserMetadataFilePath = $"{BasePermissionPath}{UserMetadataFileName}";

        // Shame train
        public static string ShameTrainChallengeDetailsFileName = "challengeDetails.txt";
        public static string ShameTrainChallengeDetailsFileNameDelimiter = "==";
        public static string ShameTrainSubscribedUsersFileName = "subscribed_users.txt";
        public static string ShameTrainSubscribedUsersFilePath = $"{ResourceDirectory}shame_train{slash}{ShameTrainSubscribedUsersFileName}";
        public static string ShameTrainSubscribedUsersFileDelimiter = ",";
        public static string ShameTrainChallengeDirectory = $"{ResourceDirectory}shame_train{slash}challenges{slash}";
        
        //Metadata Fields
        public const string ChallengeName = "ChallengeName";
        public const string ChallengeId = "ChallengeId";
        public const string ChallengeDate = "ChallengeDate";

        // Pokemon
        public static string FontDirectory = $"{ResourceDirectory}fonts{slash}";
        public static string PokemonPortraitsDirectory = $"{OutputDirectory}pokemon_portraits{slash}";
        public static string RenderedPokemonCardsDirectory = $"{OutputDirectory}pokemon_cards{slash}";

    }
}
