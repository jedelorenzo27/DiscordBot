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
   
    public class UserMetadata
    {
        private string delimiter = "{'-'}";
        private string subDelimiter = "<!>";
        private string enitlementDelimiter = "=";

        public string Name { get; set; }
        public string Id { get; set; }
        public Dictionary<Entitlement, int> requests { get; set; }


        public UserMetadata (string name, string id)
        {
            Name = name;
            Id = id;
            requests = new Dictionary<Entitlement, int> ();
        }

        public UserMetadata (string line)
        {
            string[] lineSplit = line.Split(delimiter);
            Name = lineSplit[0];
            Id = lineSplit[1];

            requests = new Dictionary<Entitlement, int> ();
            foreach (string entitlementCount in lineSplit[2].Split(subDelimiter))
            {
                string[] split = entitlementCount.Split(enitlementDelimiter);
                Entitlement entitlementOut;
                if (Enum.TryParse(split[0], out entitlementOut))
                {
                    int requestsMade = Int32.Parse(split[1]);
                    requests.Add(entitlementOut, requestsMade);
                }
            }
        }

        public void IncrementEntitlementCount(Entitlement entitlement)
        {
            if (!requests.ContainsKey(entitlement))
            {
                requests.Add(entitlement, 0);
            }
            requests[entitlement]++;


            Console.WriteLine("Writing counts");
            foreach(Entitlement key in requests.Keys)
            {
                Console.WriteLine(key + "=" + requests[key]);
            }
        }

        override
        public string ToString()
        {
            List<string> entitlementCounts = new List<string>();
            foreach(Entitlement key in requests.Keys)
            {
                entitlementCounts.Add(EnumExtensions.ToDescriptionString(key) + enitlementDelimiter + requests[key]);
            }
            Console.WriteLine(Name + delimiter + Id + delimiter + string.Join(subDelimiter, entitlementCounts));
            return Name + delimiter + Id + delimiter + string.Join(subDelimiter, entitlementCounts);
        }

    }

    public enum Entitlement
    {
        [Description("All")]
        All,
        [Description("GrantPermission")]
        GrantPermission,
        [Description("RemovePermission")]
        RemovePermission,
        [Description("ViewPermissions")]
        ViewPermissions,
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
        private static string JayUserId = "222127402980081667";
        private static HashSet<string> unbannable = new HashSet<string>() { JayUserId };

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
                return GeneralUtilities.resourceDirector + "permissions" + Path.DirectorySeparatorChar + "banned_users.txt";
            }
        }
        private static string metadataFile
        {
            get
            {
                return GeneralUtilities.resourceDirector + "permissions" + Path.DirectorySeparatorChar + "user_metadata.txt";
            }
        }

        private static Dictionary<string, HashSet<Entitlement>> permissions = new Dictionary<string, HashSet<Entitlement>>();
        private static HashSet<string> bannedUsers = new HashSet<string>();
        private static Dictionary<string, UserMetadata> userMetadata = new Dictionary<string, UserMetadata>();

        public static void LoadPermissions()
        {
            permissions = new Dictionary<string, HashSet<Entitlement>>();
            bannedUsers = new HashSet<string>();
            LoadUserPermissions();
            LoadBannedUsers();
            LoadMetadata();
        }

        public static void AddPermission(SocketInteractionContext context, string idToGiveEntitlement, Entitlement entitlement)
        {
            if (!permissions.ContainsKey(idToGiveEntitlement))
            {
                permissions.Add(idToGiveEntitlement, new HashSet<Entitlement>());
            }
            permissions[idToGiveEntitlement].Add(entitlement);
            SaveUserPermissions();
        }

        public static void RemovePermission(SocketInteractionContext context, string idToRemoveEntitlement, Entitlement entitlement)
        {
            if (permissions.ContainsKey(idToRemoveEntitlement))
            {
                permissions[idToRemoveEntitlement].Remove(entitlement);
            }
            SaveUserPermissions();
            throw new UnauthorizedException();
        }

        public static void Ban(SocketInteractionContext context, string idToBan)
        {
            if (unbannable.Contains(idToBan))
            {
                return;
            }
            bannedUsers.Add(idToBan);
            SaveBannedUsers();
        }

        public static Entitlement[] GetUserEntitlements(string id)
        {
            if (permissions.ContainsKey(id))
            {
                return permissions[id].ToArray();
            }
            return new Entitlement[0];
        }

        private static void recordPermissionCheck(string id, string name, Entitlement entitlement)
        {
            if (!userMetadata.ContainsKey(id)) 
            {
                userMetadata.Add(id, new UserMetadata(name, id));
            }
            userMetadata[id].IncrementEntitlementCount(entitlement);
            SaveMetadata();
        }
        
        public static async Task<bool> ValidatePermissions(SocketInteractionContext context, Entitlement entitlement)
        {
            recordPermissionCheck(context.User.Id.ToString(), context.User.Username, entitlement);
            recordPermissionCheck(context.Guild.Id.ToString(), context.Guild.Name, entitlement);

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

        private static void LoadUserPermissions()
        {
            try
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo's while loading user permissions file");
                Console.WriteLine(ex.Message);
            }
        }

        private static void LoadBannedUsers()
        {
            try
            {
                if (File.Exists(bannerUsersFile))
                {
                    string[] lines = File.ReadAllLines(bannerUsersFile);
                    foreach (string id in lines)
                    {
                        bannedUsers.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo's while loading banned users file");
                Console.WriteLine(ex.Message);
            }
        }

        private static void LoadMetadata()
        {
            try
            {
                if (File.Exists(metadataFile))
                {
                    string[] lines = File.ReadAllLines(metadataFile);
                    foreach (string line in lines)
                    {
                        UserMetadata metadata = new UserMetadata(line);
                        userMetadata.Add(metadata.Id, metadata);
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo's while loading user metadata file");
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveAll()
        {
            SaveUserPermissions();
            SaveBannedUsers();
            SaveMetadata();
        }

        private static void SaveUserPermissions()
        {
            try
            {
                if (File.Exists(permissionsFile))
                {
                    File.Delete(permissionsFile);
                }

                List<string> lines = new List<string>();
                foreach (string key in permissions.Keys)
                {
                    lines.Add(key + ":" + string.Join(",", permissions[key]));
                }
                File.WriteAllLines(permissionsFile, lines.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo'd while saving permissions file");
                Console.WriteLine(ex.ToString());
            }
        }

        private static void SaveBannedUsers()
        {
            try
            {
                if (File.Exists(bannerUsersFile))
                {
                    File.Delete(bannerUsersFile);
                }

                List<string> lines = new List<string>();
                foreach (string bannedId in bannedUsers)
                {
                    lines.Add(bannedId);
                }
                File.WriteAllLines(bannerUsersFile, lines.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo'd while saving banned uers file");
                Console.WriteLine(ex.ToString());
            }
        }

        private static void SaveMetadata()
        {
            try
            {
                if (File.Exists(metadataFile))
                {
                    File.Delete(metadataFile);
                }

                List<string> lines = new List<string>();
                foreach (string key in userMetadata.Keys)
                {
                    lines.Add(userMetadata[key].ToString());
                }
                File.WriteAllLines(metadataFile, lines.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo'd while saving user metadata file");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
