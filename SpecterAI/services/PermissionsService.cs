using Discord;
using Discord.Interactions;
using SpecterAI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.services
{

    public class UnauthorizedException : Exception
    {

    }
    public class BannedException : Exception
    {

    }
   
    public enum Entitlement
    {
        [Description("All")]
        All,
        [Description("GrantPermission")]
        GrantPermission,
        [Description("RemovePermission")]
        RemovePermission,
        [Description("BanOther")]
        BanOther,
        [Description("Pokemon")]
        Pokemon,
        [Description("OpenAiChat")]
        OpenAiChat,
        [Description("OpenAiImage")]
        OpenAiImage,
        
    }

    public static class PermissionsService
    {
        private static string UserJay = "222127402980081667";

        private static string permissionsFile
        {
            get
            {
                return GeneralUtilities.resourceDirector + "permissions" + Path.DirectorySeparatorChar + "permissions.txt";
            }
        }
        private static string bannerUsersFile
        {
            get
            {
                return GeneralUtilities.resourceDirector + "permissions" + Path.DirectorySeparatorChar + "usersBanned.txt";
            }
        }

        private static Dictionary<string, HashSet<Entitlement>> permissions = new Dictionary<string, HashSet<Entitlement>>();
        private static HashSet<string> bannedUsers = new HashSet<string>();

        public static void LoadPermissions()
        {
            permissions = new Dictionary<string, HashSet<Entitlement>>();
            bannedUsers = new HashSet<string>();
            Load();
        }

        public static void AddPermission(SocketInteractionContext context, string idToGiveEntitlement, Entitlement entitlement)
        {
            if (!permissions.ContainsKey(idToGiveEntitlement))
            {
                permissions.Add(idToGiveEntitlement, new HashSet<Entitlement>());
            }
            permissions[idToGiveEntitlement].Add(entitlement);
            Save();
        }

        public static void RemovePermission(SocketInteractionContext context, string idToRemoveEntitlement, Entitlement entitlement)
        {
            Save();
            throw new UnauthorizedException();
        }

        public static void Ban(SocketInteractionContext context, string idToBan)
        {
            Save();
            throw new UnauthorizedException();
        }
        
        public static async Task<bool> ValidatePermissions(SocketInteractionContext context, Entitlement entitlement)
        {
            Console.WriteLine("Checking permissions to grant");
            if (bannedUsers.Contains(context.User.Id.ToString())) {
                throw new BannedException();
            }

            if (permissions.ContainsKey(context.Guild.Id.ToString()) 
                && (permissions[context.Guild.Id.ToString()].Contains(entitlement) 
                || permissions[context.Guild.Id.ToString()].Contains(Entitlement.All)))
            {
                return true;
            }

            if (permissions.ContainsKey(context.User.Id.ToString()) 
                && (permissions[context.User.Id.ToString()].Contains(entitlement) 
                || permissions[context.User.Id.ToString()].Contains(Entitlement.All)))
            {
                return true;
            }

            Console.WriteLine("Invalid permissions for user:" + context.User.Id);
            await context.Interaction.RespondAsync("User " + context.User.Id + " is unauthorized to use " + EnumExtensions.ToDescriptionString(entitlement));
            throw new UnauthorizedException();
        }

        private static void Load()
        {
            if (File.Exists(permissionsFile))
            {
                string[] lines = File.ReadAllLines(permissionsFile);
                foreach (string line in lines)
                {
                    string[] lineSplit = line.Split(':');
                    string id = lineSplit[0];
                    string[] entitlements = lineSplit[1].Split(',');
                    permissions.Add(id, new HashSet<Entitlement>());
                    foreach (string entitlement in entitlements)
                    {
                        Entitlement result;
                        if (Enum.TryParse(entitlement, out result))
                        {
                            permissions[id].Add(result);
                        }
                    }
                }
            }   
            
            if (File.Exists(bannerUsersFile))
            {
                string[] lines = File.ReadAllLines(bannerUsersFile);
                foreach(string id in lines)
                {
                    bannedUsers.Add(id);
                    //if (permissions.ContainsKey(id))
                    //{
                    //    permissions.Remove(id);
                    //}
                }
            }
        }

        public static void Save()
        {
            if (File.Exists(permissionsFile))
            {
                File.Delete(permissionsFile);
            }

            List<string> lines = new List<string>();
            foreach(string key in permissions.Keys)
            {
                lines.Add(key + ":" + string.Join(",",permissions[key]));
            }
            File.WriteAllLines(permissionsFile, lines.ToArray());
        }
    }
}
