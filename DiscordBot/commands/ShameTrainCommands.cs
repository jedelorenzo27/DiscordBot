using Discord;
using Discord.Interactions;
using DiscordBot.services;
using SpecterAI.services;

namespace DiscordBot.commands
{
    public class ShameTrainCommands : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("shame", "Shame users for not completing the daily")]
        public async Task Shame()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.Shame);

        }

        [SlashCommand("create-challenge", "Create a new challenge thread in the-daily")]
        public async Task Create(string leetcodeURL, string challengeName, string challengeId)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.CreateChallenge);
            await ShameTrainServices.CreateDailyChallenge(Context, leetcodeURL, challengeName, challengeId);
            await RespondAsync(text: $"Created thread");
            await DeleteOriginalResponseAsync();
        }

        [SlashCommand("submit-challenge", "Submit daily-challenge solution")]
        public async Task SubmitSolution(
            [Choice("solution", "Upload")
            ] Attachment solution,

            [Choice("CSharp", "cs"), 
            Choice("Javasript", "js"), 
            Choice("Python", "py")
            ] string language,

            [Choice("O(n!)", "O(n!)"),
            Choice("O(2^n)", "O(2^n)"),
            Choice("O(n^2)", "O(n^2)"),
            Choice("O(nlogn)", "O(nlogn)"),
            Choice("O(n)", "O(n)"),
            Choice("O(logn)", "O(logn)"),
            Choice("O(n)", "O(n)")
            ] string TimeComplexity)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubmitChallenge);
            await DeferAsync();
            await LoggingService.LogMessage(LogLevel.Action, $"{PermissionsService.GetNameFromId(Context.User.Id)} submitted a solution in {PermissionsService.GetNameFromId(Context.Channel.Id)} ({PermissionsService.GetNameFromId(Context.Guild.Id)})");
            await ShameTrainServices.SubmitSolution(Context, solution, language, TimeComplexity, Context.Channel.Id, Context.User.Id);

            Action<MessageProperties> action = (x) => { x.Content = "Solution recorded! You're probably safe from the Shame Train... for now. You can verify submission by "; };
            await ModifyOriginalResponseAsync(action);
        }

        [SlashCommand("verify-submission", "Verify your submission was recorded")]
        public async Task VerifySubmission()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.VerifySubmission);
            if (await ShameTrainServices.VerifySubmission(Context.Channel.Id, Context.User.Id)) {
                await RespondAsync("You're good to go, brother. ");
            } else
            {
                await RespondAsync("We have no record of you ever submitting a solution for this challenge. ");
            }
        }



        [SlashCommand("subscribe", "subscribe to shame-train challenges")]
        public async Task Subscribe()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubscribeShameTrain);
            await ShameTrainServices.SubscribeUser(Context, Context.User.Id);
            await RespondAsync($"Welcome aboard, {Context.User.Username}. The shame train arrives everyday Monday-Friday. The only way to ensure you're not on it is to complete #the-daily challenge and submit you're solution via /submit-challenge in the challenge thread. ");
        }

        [SlashCommand("unsubscribe", "unsubscribe from shame-train challenges - you may be shamed one last time")]
        public async Task Unsubscribe(
            [Choice("I'm employed!", "0"),
            Choice("I don't need to practice for interviews", "1"),
            Choice("I desire shame!", "2"),
            Choice("I desire shame!", "3"),
            ] string reason)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.UnsubscribeShameTrain);
            await ShameTrainServices.UnsubscribeUser(Context, Context.User.Id);
            await RespondAsync($"Missing messages of shame. You're off the hook this time, {Context.User.Username}. Now get out of here you little scamp!");
        }

        [SlashCommand("submissions", "View submissions for a given thread")]
        public async Task ViewSubmissions()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.ViewChallengeSubmissions);
        }

    }
}
