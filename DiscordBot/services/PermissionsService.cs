﻿using Discord;
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
        }

        override
        public string ToString()
        {
            List<string> entitlementCounts = new List<string>();
            foreach(Entitlement key in requests.Keys)
            {
                entitlementCounts.Add(EnumExtensions.ToDescriptionString(key) + enitlementDelimiter + requests[key]);
            }
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
        [Description("ViewUsage")]
        ViewUsage,
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
        private static string JonathanUserId = "429310221861519374";
        private static string ChrisUserId = "447113923162800148";
        private static string globalId = "global_access";
        private static HashSet<string> unbannable = new HashSet<string>() { globalId, JayUserId, JonathanUserId, ChrisUserId };

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

        private static Dictionary<string, HashSet<Entitlement>> _permissions = new Dictionary<string, HashSet<Entitlement>>();
        private static HashSet<string> _bannedUsers = new HashSet<string>();
        private static Dictionary<string, UserMetadata> _userMetadata = new Dictionary<string, UserMetadata>();
        private static Random _random = new Random();
        public static void LoadPermissions()
        {
            _permissions = new Dictionary<string, HashSet<Entitlement>>();
            _bannedUsers = new HashSet<string>();
            LoadUserPermissions();
            LoadBannedUsers();
            LoadMetadata();
        }

        public static string GetNameFromId(string user_id)
        {
            if (_userMetadata.ContainsKey(user_id))
            {
                return _userMetadata[user_id].Name;
            }
            return user_id;
        }

        public static async Task GrantPermission(SocketInteractionContext context, string idToGiveEntitlement, Entitlement entitlement)
        {
            if (!_permissions.ContainsKey(idToGiveEntitlement))
            {
                await LoggingService.LogMessage(LogLevel.Info, $"Granting permission ({EnumExtensions.ToDescriptionString(entitlement)}) to user:{GetNameFromId(idToGiveEntitlement)}");
                _permissions.Add(idToGiveEntitlement, new HashSet<Entitlement>());
            } else
            {
                await LoggingService.LogMessage(LogLevel.Info, $"user:{GetNameFromId(idToGiveEntitlement)} already has ${EnumExtensions.ToDescriptionString(entitlement)}");
            }
            _permissions[idToGiveEntitlement].Add(entitlement);
            SaveUserPermissions();
        }

        public static async Task RemovePermission(SocketInteractionContext context, string idToRemoveEntitlement, Entitlement entitlement)
        {
            if (_permissions.ContainsKey(idToRemoveEntitlement))
            {
                // Only another unbannable can revoke the permissions of an unbannable (for debug purposes)
                if (unbannable.Contains(idToRemoveEntitlement) && !unbannable.Contains(context.User.Id.ToString()))
                {
                    await LoggingService.LogMessage(LogLevel.Info, $"{GetNameFromId(context.User.Id.ToString())} ({context.User.Id.ToString()}) tried to revoke permissions from {GetNameFromId(idToRemoveEntitlement)}");
                } else
                {
                    await LoggingService.LogMessage(LogLevel.Info, $"Removing permission ({EnumExtensions.ToDescriptionString(entitlement)}) from user:{GetNameFromId(idToRemoveEntitlement)}");
                    _permissions[idToRemoveEntitlement].Remove(entitlement);
                    SaveUserPermissions();
                }
            } else
            {
                await LoggingService.LogMessage(LogLevel.Info, $"Could not remove permission as (user:{GetNameFromId(idToRemoveEntitlement)}) does not have ${EnumExtensions.ToDescriptionString(entitlement)}");
            }
            
        }

        public static async Task Ban(SocketInteractionContext context, string idToBan)
        {
            if (unbannable.Contains(idToBan))
            {
                await LoggingService.LogMessage(LogLevel.Info, $"A user ({GetNameFromId(context.User.Id.ToString())}:{context.User.Id}) tried to ban an unbannable ({GetNameFromId(idToBan)}) user");
                await context.Interaction.RespondAsync(GetDeniedMessageForBanningUnbannable(context.User.Id.ToString()));
                return;
            }
            await LoggingService.LogMessage(LogLevel.Info, $"Banning user: {GetNameFromId(idToBan)}");
            _bannedUsers.Add(idToBan);
            SaveBannedUsers();
        }

        public static Entitlement[] GetUserEntitlements(string id)
        {
            if (_permissions.ContainsKey(id))
            {
                return _permissions[id].ToArray();
            }
            return new Entitlement[0];
        }

        public static Dictionary<Entitlement, int>? GetEntitlementUsage(SocketInteractionContext context, string id)
        {
            if (_userMetadata.ContainsKey(id))
            {
                return _userMetadata[id].requests;
            }
            return null;
        }

        private static void recordPermissionCheck(string id, string name, Entitlement entitlement)
        {
            if (!_userMetadata.ContainsKey(id)) 
            {
                _userMetadata.Add(id, new UserMetadata(name, id));
            }
            _userMetadata[id].IncrementEntitlementCount(entitlement);
            SaveMetadata();
        }

        public static async Task<bool> ValidatePermissions(SocketInteractionContext context, Entitlement entitlement)
        {
            recordPermissionCheck(context.User.Id.ToString(), context.User.Username, entitlement);
            recordPermissionCheck(context.Guild.Id.ToString(), context.Guild.Name, entitlement);

            if (_bannedUsers.Contains(context.User.Id.ToString())) {
                await LoggingService.LogMessage(LogLevel.Info, $"{GetNameFromId(context.User.Id.ToString())} failed entitlement check for '{entitlement}' due to being banned.");
                await context.Interaction.RespondAsync(GetDeniedMessageForBannedUser(context.User.Id.ToString()));
                throw new BannedException();
            }

            if (_permissions.ContainsKey(globalId) && _permissions[globalId].Contains(entitlement)) {
                return true;
            }

            if (_permissions.ContainsKey(context.Guild.Id.ToString())
                && (_permissions[context.Guild.Id.ToString()].Contains(entitlement)
                || _permissions[context.Guild.Id.ToString()].Contains(Entitlement.All)))
            {
                return true;
            }

            if (_permissions.ContainsKey(context.User.Id.ToString())
                && (_permissions[context.User.Id.ToString()].Contains(entitlement)
                || _permissions[context.User.Id.ToString()].Contains(Entitlement.All)))
            {
                return true;
            }
            await LoggingService.LogMessage(LogLevel.Info, $"{GetNameFromId(context.User.Id.ToString())} failed entitlement check for '{entitlement}'");
            await context.Interaction.RespondAsync(GetDeniedMessageForUnauthrorizedUser(context.User.Id.ToString()));
            throw new UnauthorizedException();
        }

        private static string GetDeniedMessageForUnauthrorizedUser(string userId)
        {
            string[] possibleResponse = new string[] {
                $"Come on, {GetNameFromId(userId)}. You're not fooling anyone around here. We both know you ain't got the permissions do that.",
                "Say please",
                "Try asking someone for more permissions or granting them to yourself",
                $"{GetNameFromId(userId)}, ain't you learned by now you cain't do that",
                $"{GetNameFromId(userId)}, no.",
                "No.",
                "!! Access denied !!",
                "Unauthorized access has been reported and logged",
                "You're not yet strong enough to do that",
                "You'll have to grow stronger",
                "erm, lets not continue this relationship.",
                "Cancelling request",
                "It'd be better if you didn't do that",
                "We've thought about this long and hard and come to the conclusion that you're just not ready for this kind of responsibility.",
                "Sorry bud, can't do that",
                $"{GetNameFromId(userId)}, please don't make this harder than it needs to be",
                "Lol, is this Xander?"
            };

            return possibleResponse[_random.Next(possibleResponse.Length)];
            
        }

        private static string GetDeniedMessageForBannedUser(string userId)
        {
            string[] possibleResponse = new string[] {
                "You really be out here tryna make calls when you know you're banned?",
                "Ur kind ain't welcome round these parts, pardner",
                $"Is that {GetNameFromId(userId)}? Nah, can't be. This dude knows he's banned yet still comin' around.",
                "Shame",
                "Sorry, I don't associate with BANNED USERS",
                "Ew, a disgusting banned user.",
                "Eek! A banned user!",
                $"Oh god, it's {GetNameFromId(userId)} again...",
                "How many times I gotta tell this guy..."
            };
            return possibleResponse[_random.Next(possibleResponse.Length)];
        }

        private static string GetDeniedMessageForBanningUnbannable(string userId)
        {
            string[] possibleResponse = new string[] {
                "lol",
                "Yeah sure, 'Done.'",
                "*awink*"
            };
            return possibleResponse[_random.Next(possibleResponse.Length)];
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
                        _permissions.Add(id, new HashSet<Entitlement>());
                        foreach (string entitlement in entitlements)
                        {
                            Entitlement result;
                            if (Enum.TryParse(entitlement, out result))
                            {
                                _permissions[id].Add(result);
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

            foreach (string id in unbannable)
            {
                if (!_permissions.ContainsKey(id))
                {
                    _permissions.Add(id, new HashSet<Entitlement>());
                }
                _permissions[id].Add(Entitlement.GrantPermission);
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
                        _bannedUsers.Add(id);
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
                        _userMetadata.Add(metadata.Id, metadata);
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
                foreach (string key in _permissions.Keys)
                {
                    lines.Add(key + ":" + string.Join(",", _permissions[key]));
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
                foreach (string bannedId in _bannedUsers)
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
                foreach (string key in _userMetadata.Keys)
                {
                    lines.Add(_userMetadata[key].ToString());
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
