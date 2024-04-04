using BotShared.models;
using Discord.Commands;
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
        [SlashCommand("permissions-view", "Returns a user's granted permissions")]
        public async Task ViewPermissions(string user_id)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.ViewPermissions);

            Entitlement[] entitlements = await PermissionsService.GetUserEntitlements(user_id);
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
            [Choice("GrantPermission", "GrantPermission")]
            [Choice("RemovePermission", "RemovePermission")]
            [Choice("ViewPermissions", "ViewPermissions")]
            [Choice("ViewUsage", "ViewUsage")]
            [Choice("BanOther", "BanOther")]
            [Choice("Pokemon", "Pokemon")]
            [Choice("OpenAiChat", "OpenAiChat")]
            [Choice("OpenAiImage", "OpenAiImage")]
            [Choice("Shame", "Shame")]
            [Choice("CreateChallenge", "CreateChallenge")]
            [Choice("DeleteChallenge", "DeleteChallenge")]
            [Choice("SubmitChallenge", "SubmitChallenge")]
            [Choice("VerifySubmission", "VerifySubmission")]
            [Choice("SubscribeShameTrain", "SubscribeShameTrain")]
            [Choice("UnsubscribeShameTrain", "UnsubscribeShameTrain")]
            [Choice("ViewChallengeSubmissions", "ViewChallengeSubmissions")]
            string permission)
        {
            Console.WriteLine("Granting permission");
            await PermissionsService.ValidatePermissions(Context, Entitlement.GrantPermission);
            Console.WriteLine("Passed permission check");
            Entitlement result;
            if (Enum.TryParse(permission, out result))
            {
                await PermissionsService.GrantPermission(Context, id, result);
                await RespondAsync("Done");
            } else
            {
                await RespondAsync(permission + " is not a valid permission");
            }
        }

        //TODO: wildly inefficient - write a method for bulk adds
        [SlashCommand("permissions-grant-all", "Grants a user or server a permission")]
        public async Task GrantAllPermissions(string id)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.GrantPermission);
            foreach (Entitlement entitlement in Enum.GetValues(typeof(Entitlement)).Cast<Entitlement>())
            {
                await PermissionsService.GrantPermission(Context, id, entitlement);
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

        //TODO: wildly inefficient - write a method for bulk deletes
        [SlashCommand("permissions-revoke-all", "Grants a user or server a permission")]
        public async Task RevokeAllPermissions(string id)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.GrantPermission);
            foreach (Entitlement entitlement in Enum.GetValues(typeof(Entitlement)).Cast<Entitlement>())
            {
                await PermissionsService.RemovePermission(Context, id, entitlement);
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
