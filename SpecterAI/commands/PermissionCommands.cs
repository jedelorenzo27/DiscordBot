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
            Console.WriteLine(Context.User.Id);
            string response = "Available Permissions: ";
            List<Entitlement> entitlements = Enum.GetValues(typeof(Entitlement)).Cast<Entitlement>().ToList();
            foreach (Entitlement entitlement in entitlements)
            {
                response += "\n" + EnumExtensions.ToDescriptionString(entitlement);
            }
            await RespondAsync(response);
        }


        [SlashCommand("permissions-grant", "Grants a user or server a permission")]
        public async Task GrantPermission(string id, string permission)
        {
            Console.WriteLine(Context.User.Id + " is trying to grant permissions");
            await PermissionsService.ValidatePermissions(Context, Entitlement.GrantPermission);
            
            Entitlement result;
            if (Enum.TryParse<Entitlement>(permission, out result))
            {
                PermissionsService.AddPermission(Context, id, result);
                await RespondAsync("Done");
            } else
            {
                await RespondAsync(permission + " is not a valid permission");
            }
        }
    }
}
