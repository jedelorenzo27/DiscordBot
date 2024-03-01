using Discord.Interactions;
using SpecterAI.services;
using SpecterAI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.commands
{
    public class PermissionCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("permissions-list", "Grants a user or server a permission")]
        public async Task ListPermissions()
        {
            string response = "Available Permissions: ";
            List<Entitlement> entitlements = Enum.GetValues(typeof(Entitlement)).Cast<Entitlement>().ToList();
            foreach (Entitlement entitlement in entitlements)
            {
                response += "\n" + EnumExtensions.ToDescriptionString(entitlement);
            }
            await RespondAsync(response);
        }

        [SlashCommand("permissions-view", "Returns a user's granted permissions")]
        public async Task ViewPermissions(string user_id)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.ViewPermissions);

            Entitlement[] entitlements = PermissionsService.GetUserEntitlements(user_id);
            string response = "";
            if (entitlements.Length > 0)
            {
                response = $"User ({PermissionsService.GetNameFromId(user_id)}) has the following permissions: " + string.Join(", ", entitlements);
            } else
            {
                response = $"{PermissionsService.GetNameFromId(user_id)} is powerless";
            }
            await RespondAsync(response);
        }

        [SlashCommand("permissions-view-usage", "Returns usage stats for a given user.")]
        public async Task ViewUsage(string user_id)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.ViewUsage);
            Dictionary<Entitlement, int>? usage = PermissionsService.GetEntitlementUsage(Context, user_id);
            string response = "";
            if (usage != null)
            {
                response = $"Usage for {PermissionsService.GetNameFromId(user_id)}";
                foreach (Entitlement entitlement in usage.Keys)
                {
                    response += $"\n{EnumExtensions.ToDescriptionString(entitlement)} : {usage[entitlement]}";
                }
            } else
            {
                response = $"No usage found for {PermissionsService.GetNameFromId(user_id)}";
            }
            await RespondAsync(response);
        }

        [SlashCommand("permissions-grant", "Grants a user or server a permission")]
        public async Task GrantPermission(string id,
            [Choice("All", "All")]
            [Choice("GrantPermission", "GrantPermission")]
            [Choice("RemovePermission", "RemovePermission")]
            [Choice("ViewPermissions", "ViewPermissions")]
            [Choice("ViewUsage", "ViewUsage")]
            [Choice("BanOther", "BanOther")]
            [Choice("Pokemon", "Pokemon")]
            [Choice("OpenAiChat", "OpenAiChat")]
            [Choice("OpenAiImage", "OpenAiImage")]
            string permission)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.GrantPermission);
            Entitlement result;

            if (Enum.TryParse<Entitlement>(permission, out result))
            {
                await PermissionsService.GrantPermission(Context, id, result);
                await RespondAsync("Done");
            } else
            {
                await RespondAsync(permission + " is not a valid permission");
            }
        }

        [SlashCommand("permissions-revoke", "Revokes a users permissions")]
        public async Task RevokePermission(string id, string permission)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.RemovePermission);

            Entitlement result;
            if (Enum.TryParse<Entitlement>(permission, out result))
            {
                await PermissionsService.RemovePermission(Context, id, result);
                await RespondAsync("Done");
            }
            else
            {
                await RespondAsync(permission + " is not a valid permission");
            }
        }

        [SlashCommand("permissions-ban", "Bans a user")]
        public async Task Ban(string id)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.BanOther);
            await PermissionsService.Ban(Context, id);
            await RespondAsync("Done");
        }
    }
}
