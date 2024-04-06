using BotShared.models;
using Discord;
using Discord.Interactions;
using BotShared.Utilities;
using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.commands
{
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("admin-version", "Returns the current running version")]
        public async Task GetVersion()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.Admin);
            await DeferAsync();
            Action<MessageProperties> action = (x) => { x.Content = $"Current version: {Constants.BotVersion}"; };
            await ModifyOriginalResponseAsync(action);
        }


        [SlashCommand("admin-delete-all", "Deletes all db data")]
        public async Task DeleteDbData()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.AdminDeleteData);
            await DeferAsync();

            await Program._adminRepo.DeleteAllData();

            Action<MessageProperties> action = (x) => { x.Content = $"Dooooone "; };
            await ModifyOriginalResponseAsync(action);
        }
    }
}
