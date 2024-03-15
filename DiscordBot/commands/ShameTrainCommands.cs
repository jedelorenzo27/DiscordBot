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
            Console.WriteLine("Shaming");
            await PermissionsService.ValidatePermissions(Context, Entitlement.Shame);
            //await DeferAsync();
            await LoggingService.LogCommandUse(Context, "shame");

            Dictionary<ulong, int>  allUsers = await ShameTrainServices.UserIdsToDaysSinceLastSubmission(Context);
            Dictionary<ulong, int> usersNeedingShaming = new Dictionary<ulong, int>();
            List<ulong> usersSafeFromShame = new List<ulong>();

            Console.WriteLine("Shaming users");
            foreach(ulong userId in allUsers.Keys)
            {
                Console.WriteLine($"{userId}: {allUsers[userId]}");
                if (allUsers[userId] == 0)
                {
                    usersSafeFromShame.Add(userId);
                } else
                {
                   usersNeedingShaming.Add(userId, allUsers[userId]);
                }
            }

            List<string> messageLines = new List<string>();
            messageLines.Add(":shame_badge: Shame Train is Arriving :shame_badge:");

            foreach(ulong user in usersNeedingShaming.Keys) 
            {
                messageLines.Add($"<@{user}> {usersNeedingShaming[user]}");
            }

            if (usersSafeFromShame.Count > 0)
            {
                messageLines.Add("----------------------");
                messageLines.Add("SAFE");

                foreach (ulong user in usersSafeFromShame)
                {
                    messageLines.Add($":shame_train:<@{user}>");
                }
            } else
            {
                messageLines.Add($"No one was saved.");
            }
            Console.WriteLine("Sending discord message of shame");

            await RespondAsync(string.Join("\n", messageLines.ToArray()));
        }

        [SlashCommand("create-challenge", "Create a new challenge thread in the-daily")]
        public async Task Create(string leetcodeURL, 
            [Choice("Easy", "Easy"),
            Choice("Medium", "Medium"),
            Choice("Hard", "Hard")
            ] string difficulty, string challengeName, string challengeId)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.CreateChallenge);
            await LoggingService.LogCommandUse(Context, "create-challenge");
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
            ] string timeComplexity)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubmitChallenge);
            await DeferAsync();
            await LoggingService.LogCommandUse(Context, "submit-challenge");
            string[] solutionLines = await ShameTrainServices.SubmitSolution(Context, solution, language, timeComplexity, Context.Channel.Id, Context.User.Id);

            List<string> responseLines = new List<string>();
            responseLines.Add($"Thanks for the submission, ${PermissionsService.GetNameFromId(Context.User.Id)}! You're probably safe from the Shame Train... for now. You can verify submission via /verify-submission ");
            responseLines.Add("||```");
            foreach(string line in solutionLines)
            {
                string sanitizedLine = line.Replace("||", "OR");
                responseLines.Add(sanitizedLine);
            }
            responseLines.Add("```||");
            responseLines.Add($"Completed in {timeComplexity}");

            Action<MessageProperties> action = (x) => { x.Content = string.Join("\n", responseLines.ToArray()); };
            await ModifyOriginalResponseAsync(action);
        }

        [SlashCommand("verify-submission", "Verify your submission was recorded")]
        public async Task VerifySubmission()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.VerifySubmission);
            await LoggingService.LogCommandUse(Context, "verify-submission");

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
            await LoggingService.LogCommandUse(Context, "subscribe");
            await ShameTrainServices.SubscribeUser(Context, Context.User.Id);
            await RespondAsync($"Welcome aboard, {Context.User.Username}. The shame train arrives everyday Monday-Friday. The only way to ensure you're not on it is to complete #the-daily challenge and submit you're solution via /submit-challenge in the challenge thread. ");
        }

        [SlashCommand("unsubscribe", "unsubscribe from shame-train challenges - you may be shamed one last time")]
        public async Task Unsubscribe(
            [Choice("I'm employed!", 0),
            Choice("I don't need to practice for interviews", 1),
            Choice("I desire shame!", 2),
            Choice("I desire shame!", 3),
            ] int reason)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.UnsubscribeShameTrain);
            await LoggingService.LogCommandUse(Context, "unsubscribe");
            await ShameTrainServices.UnsubscribeUser(Context, Context.User.Id);
            await RespondAsync($"Missing messages of shame. You're off the hook this time, {Context.User.Username}. Now get out of here you little scamp!");
        }

        [SlashCommand("view-submissions", "View submissions for a given thread")]
        public async Task ViewSubmissions()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.ViewChallengeSubmissions);
            await LoggingService.LogCommandUse(Context, "submissions");

        }

    }
}
