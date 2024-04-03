using BotShared.models;
using Discord;
using Discord.Interactions;
using DiscordBot.Utilities;
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

    public static class PermissionsService
    {
        
        private static string globalId = "global_access";
        private static HashSet<string> unbannable = new HashSet<string>() { globalId, Constants.JayUserId, Constants.JonathanUserId, Constants.ChrisUserId };

        private static HashSet<string> _bannedUsers = new HashSet<string>();
        private static Dictionary<string, UserMetadata> _userMetadata = new Dictionary<string, UserMetadata>();
        private static Random _random = new Random();
        public static async Task LoadPermissions()
        {
            //TODO: setup banned users
            _bannedUsers = new HashSet<string>();
            await Program._entitlementRepo.AddEntitlement(Constants.JayUserId, Entitlement.GrantPermission);
            await Program._entitlementRepo.AddEntitlement(Constants.JonathanUserId, Entitlement.GrantPermission);
        }

        public static string GetNameFromId(ulong user_id)
        {
            return GetNameFromId(user_id.ToString());  
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
            await LoggingService.LogMessage(LogLevel.Info, $"Granting permission ({EnumExtensions.ToDescriptionString(entitlement)}) to user:{GetNameFromId(idToGiveEntitlement)}");
            await Program._entitlementRepo.AddEntitlement(idToGiveEntitlement, entitlement);
        }

        public static async Task RemovePermission(SocketInteractionContext context, string idToRemoveEntitlement, Entitlement entitlement)
        {
            await LoggingService.LogMessage(LogLevel.Info, $"Removing permission ({EnumExtensions.ToDescriptionString(entitlement)}) from user:{GetNameFromId(idToRemoveEntitlement)}");
            await Program._entitlementRepo.RemoveEntitlement(idToRemoveEntitlement, entitlement);            
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
        }

        public static async Task<Entitlement[]> GetUserEntitlements(string id)
        {
            List< EntitlementModel> entitlementModels = await Program._entitlementRepo.GetEntitlementsById(id);
            if (entitlementModels != null)
            {
                Entitlement[] permissions = new Entitlement[entitlementModels.Count];
                for(int i  = 0; i < entitlementModels.Count; i++)
                {
                    permissions[i] = entitlementModels[i].Entitlement;
                }
                return permissions;
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

        private static async Task recordPermissionCheck(string id, string name, Entitlement entitlement)
        {
            if (!_userMetadata.ContainsKey(id)) 
            {
                _userMetadata.Add(id, new UserMetadata(name, id));
            }
            _userMetadata[id].IncrementEntitlementCount(entitlement);
        }

        public static async Task<bool> ValidatePermissions(SocketInteractionContext context, Entitlement entitlement)
        {
            await recordPermissionCheck(context.User.Id.ToString(), context.User.Username, entitlement);
            await recordPermissionCheck(context.Channel.Id.ToString(), context.Channel.Name, entitlement);
            await recordPermissionCheck(context.Guild.Id.ToString(), context.Guild.Name, entitlement);

            if (_bannedUsers.Contains(context.User.Id.ToString())) {
                await LoggingService.LogMessage(LogLevel.Info, $"{GetNameFromId(context.User.Id.ToString())} failed entitlement check for '{entitlement}' due to being banned.");
                await context.Interaction.RespondAsync(GetDeniedMessageForBannedUser(context.User.Id.ToString()));
                throw new BannedException();
            }

            if (await Program._entitlementRepo.GetEntitlementsByIds(new string[] { context.Guild.Id.ToString(), context.Channel.Id.ToString(), context.User.Id.ToString() }, entitlement) != null)
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
    }
}
