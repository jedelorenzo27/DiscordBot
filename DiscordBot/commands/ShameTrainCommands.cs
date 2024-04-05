using BotShared.models;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using DiscordBot.services;
using SpecterAI.services;
using SpecterAI.Utilities;
using System.Security;

namespace DiscordBot.commands
{
    public class ShameTrainCommands : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("shame", "Shame users for not completing the daily")]
        public async Task Shame()
        {
            Console.WriteLine("Shaming");
            await PermissionsService.ValidatePermissions(Context, Entitlement.Shame);
            await DeferAsync();
            await LoggingService.LogCommandUse(Context, "shame");

            ShameStats shameStats = await ShameTrainServices.GetShameStats(Context);
            List<string> messageLines = new List<string>()
            {
                $"All aboard shame train #{shameStats.totalChallenges}!"
            };

            List<(string userId, int missedChallenges)> countSinceLastSubmission = new List<(string userId, int missedChallenges)>(shameStats.countSinceLastSubmission);
            countSinceLastSubmission.Reverse();
            while (countSinceLastSubmission.Count > 0 && countSinceLastSubmission[0].missedChallenges > 1)
            {
                messageLines.Add($"<@{countSinceLastSubmission[0].userId}> failed to complete the last {countSinceLastSubmission[0].missedChallenges} challenges.");
                countSinceLastSubmission.RemoveAt(0);
            }

            while (countSinceLastSubmission.Count > 0 && countSinceLastSubmission[0].missedChallenges > 0)
            {
                messageLines.Add($"<@{countSinceLastSubmission[0].userId}> failed to complete the last challenge.");
                countSinceLastSubmission.RemoveAt(0);
            }
            messageLines.Add("---------------------------------");

            if (countSinceLastSubmission.Count > 0)
            {
                messageLines.Add("The following users are SAFE. Great job out there!");
                while (countSinceLastSubmission.Count > 0)
                {
                    messageLines.Add($"<@{countSinceLastSubmission[0].userId}>");
                    countSinceLastSubmission.RemoveAt(0);
                }
            } else
            {
                messageLines.Add("Ok, the shame train is here. Let's go, everyone on.");
            }
            //await RespondAsync(string.Join("\n", messageLines.ToArray()));
            Action<MessageProperties> action = (x) => { x.Content = string.Join("\n", messageLines.ToArray()); };
            await ModifyOriginalResponseAsync(action);
        }

        [SlashCommand("create-challenge", "Create a new challenge thread in the-daily")]
        public async Task Create(string leetcodeURL, 
            [Choice("Easy", "Easy"),
            Choice("Medium", "Medium"),
            Choice("Hard", "Hard")
            ] string difficulty, string challengeName, string challengeId)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.CreateChallenge);
            await DeferAsync();
            await LoggingService.LogCommandUse(Context, "create-challenge");
            await ShameTrainServices.CreateDailyChallenge(Context, leetcodeURL, challengeName, int.Parse(challengeId));
            await DeleteOriginalResponseAsync();
        }


        [SlashCommand("submit-challenge", "Submit daily-challenge solution")]
        public async Task SubmitSolution(
            [Choice("CSharp", "CSharp"),
            Choice("Javascript", "Javascript"),
            Choice("Python", "Python")
            ] string language,

            [Choice("O(n!)", "N_Factorial"),
            Choice("O(2^n)", "Two_To_The_N"),
            Choice("O(n^2)", "M_Squared"),
            Choice("O(nlogn)", "N_Log_N"),
            Choice("O(n)", "N"),
            Choice("O(logn)", "Log_N"),
            Choice("O(1)", "Constant")
            ] string timeComplexity)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubmitChallenge);
            await DeferAsync();
            await LoggingService.LogCommandUse(Context, "submit-challenge");

            TimeComplexity timeComplexityResult = (TimeComplexity)Enum.Parse(typeof(TimeComplexity), timeComplexity);
            Language languageResult = (Language)Enum.Parse(typeof(Language), language);

            string[] solutionLines = await ShameTrainServices.SubmitSolution(Context, null, languageResult, timeComplexityResult);

            List<string> responseLines = new List<string>
            {
                $"Thanks for the submission, <@{Context.User.Id}>! You're probably safe from the Shame Train... for now",
                $"Solution completes in O({EnumExtensions.ToDescriptionString(timeComplexityResult)})"
            };

            Action<MessageProperties> action = (x) => { x.Content = string.Join("\n", responseLines.ToArray()); };
            await ModifyOriginalResponseAsync(action);
        }


        [SlashCommand("submit-challenge-file", "Submit daily-challenge solution with file")]
        public async Task SubmitSolution(
            [Choice("solution", "Upload")
            ] Attachment solution,

            [Choice("CSharp", "CSharp"),
            Choice("Javascript", "Javascript"),
            Choice("Python", "Python")
            ] string language,

            [Choice("O(n!)", "N_Factorial"),
            Choice("O(2^n)", "Two_To_The_N"),
            Choice("O(n^2)", "M_Squared"),
            Choice("O(nlogn)", "N_Log_N"),
            Choice("O(n)", "N"),
            Choice("O(logn)", "Log_N"),
            Choice("O(1)", "Constant")
            ] string timeComplexity)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubmitChallenge);
            await DeferAsync();
            await LoggingService.LogCommandUse(Context, "submit-challenge");

            TimeComplexity timeComplexityResult = (TimeComplexity)Enum.Parse(typeof(TimeComplexity), timeComplexity);
            Language languageResult = (Language)Enum.Parse(typeof(Language), language);

            string[] solutionLines = await ShameTrainServices.SubmitSolution(Context, solution, languageResult, timeComplexityResult);

            List<string> responseLines = new List<string>
            {
                $"Thanks for the submission, <@{Context.User.Id}>! You're probably safe from the Shame Train... for now",
                "||```"
            };
            foreach(string line in solutionLines)
            {
                string sanitizedLine = line.Replace("||", "OR");
                responseLines.Add(sanitizedLine);
            }
            responseLines.Add("```||");

            responseLines.Add($"Solution completes in O({EnumExtensions.ToDescriptionString(timeComplexityResult)})");



            Action<MessageProperties> action = (x) => { x.Content = string.Join("\n", responseLines.ToArray()); };
            await ModifyOriginalResponseAsync(action);
        }

        /*[SlashCommand("verify-submission", "Verify your submission was recorded")]
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
        }*/

        [SlashCommand("subscribe", "subscribe to shame-train challenges")]
        public async Task Subscribe()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubscribeShameTrain);
            await LoggingService.LogCommandUse(Context, "subscribe");
            await ShameTrainServices.SubscribeUser(Context, Context.Guild.Id.ToString(), Context.User.Id.ToString());
            await RespondAsync($"Welcome aboard, {Context.User.Username}. The shame train arrives everyday Monday-Friday. The only way to ensure you're not on it is to complete #the-daily challenge and submit you're solution via /submit-challenge in the challenge thread. ");
        }

        [SlashCommand("subscribe-other", "subscribe to shame-train challenges")]
        public async Task SubscribeOther(string userId)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.SubscribeOtherToShameTrain);
            await LoggingService.LogCommandUse(Context, "subscribe");
            await ShameTrainServices.SubscribeUser(Context, Context.Guild.Id.ToString(), userId);
            await RespondAsync($"Welcome aboard, {userId}. The shame train arrives everyday Monday-Friday. The only way to ensure you're not on it is to complete #the-daily challenge and submit you're solution via /submit-challenge in the challenge thread. ");
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
            await ShameTrainServices.UnsubscribeUser(Context, Context.Guild.Id.ToString(), Context.User.Id.ToString());
            await RespondAsync($"Missing messages of shame. You're off the hook this time, {Context.User.Username}. Now get out of here you little scamp!");
        }

        /*[SlashCommand("view-submissions", "View submissions for a given thread")]
        public async Task ViewSubmissions()
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.ViewChallengeSubmissions);
            await LoggingService.LogCommandUse(Context, "submissions");

        }*/


        [SlashCommand("backfill-challenge", "backfill")]
        public async Task BackfillChallenge(int leetcodeNumber, string submittedUsers)
        {
            await PermissionsService.ValidatePermissions(Context, Entitlement.BackfillChallenge);
            await LoggingService.LogCommandUse(Context, "backfill-challenge");
            await ShameTrainServices.BackfillChallenge(Context, leetcodeNumber, submittedUsers.Split(','));
            await RespondAsync($"Done");
        }
    }
}
