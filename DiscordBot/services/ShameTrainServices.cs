using Discord;
using Discord.Interactions;
using SpecterAI.services;
using DiscordBot.Utilities;
using SpecterAI.Utilities;
using Version = DiscordBot.Utilities.Version;
using BotShared.models;
using System.Threading.Channels;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.services
{
    public class ShameStats
    {
        public List<(string userId, int missedChallenges)> countSinceLastSubmission;
        public int totalChallenges;
    }

    public static class ShameTrainServices
    {

        public static async Task<ShameStats> GetShameStats(SocketInteractionContext Context)
        {
            List<ChallengeModel> challenges = await Program._challengeRepo.GetChallengesByServerId(Context.Guild.Id.ToString());
            challenges.Sort((ch1, ch2) => DateTime.Compare(ch1.CreationDate, ch2.CreationDate));
            challenges.Reverse();

            List<ChallengeSubscriberModel> currentSubscribers = await Program._challengeSubscriberRepo.GetSubscribersByDiscordId(Context.Guild.Id.ToString());

            Dictionary<string, (ChallengeModel challenge, List<ChallengeSubmissionModel> submissions)> challengeIdToSubmissions = new Dictionary<string, (ChallengeModel challenge, List<ChallengeSubmissionModel> submissions)>();
            foreach (ChallengeModel challenge in challenges)
            {
                HashSet<string> seenUserIds = new HashSet<string>();

                List<ChallengeSubmissionModel> submissions = await Program._challengeSubmissionRepo.GetSubmissionsForChallenge(challenge.ChallengeId);
                // remove multiple submissions since we only care about whether they have at least one submission
                for(int i = submissions.Count - 1; i >= 0; i--)
                {
                    if (seenUserIds.Contains(submissions[i].UserId))
                    {
                        submissions.RemoveAt(i);
                    } else
                    {
                        seenUserIds.Add(submissions[i].UserId);
                    }
                }
                challengeIdToSubmissions.Add(challenge.ChallengeId, (challenge, submissions));
            }

            List<(string userId, int missedChallenges)> missedChallengeCounts = await CountChallengesSinceLastSubmission(challengeIdToSubmissions, currentSubscribers);
            return new ShameStats()
            {
                countSinceLastSubmission = missedChallengeCounts,
                totalChallenges = challenges.Count,
            };
        }

        private static async Task<List<(string userId, int missedChallenges)>> CountChallengesSinceLastSubmission(Dictionary<string, (ChallengeModel challenge, List<ChallengeSubmissionModel> submissions)> challengeIdToSubmissions, List<ChallengeSubscriberModel> currentSubscribers)
        {
            List<(string userId, int missedChallenges)> missedChallengeCounts = new List<(string userId, int missedChallenges)>();
            int missedChallengeCount = 0;
            HashSet<string> subscribers = SubscribersModelToIdSet(currentSubscribers);
            foreach (string challengeId in challengeIdToSubmissions.Keys)
            {
                foreach (ChallengeSubmissionModel submission in challengeIdToSubmissions[challengeId].submissions)
                {
                    if (subscribers.Contains(submission.UserId))
                    {
                        missedChallengeCounts.Add((submission.UserId, missedChallengeCount));
                        subscribers.Remove(submission.UserId);
                    }
                }
                missedChallengeCount++;
            }

            foreach (string subscriberId in subscribers)
            {
                missedChallengeCounts.Add((subscriberId, missedChallengeCount));
            }
            return missedChallengeCounts;
        }

        private static HashSet<string> SubscribersModelToIdSet(List<ChallengeSubscriberModel> subscribers)
        {
            HashSet<string> result = new HashSet<string>();
            foreach(ChallengeSubscriberModel subscriber in subscribers)
            {
                result.Add(subscriber.UserId);
            }
            return result;
        }


        public static async Task BackfillChallenge(SocketInteractionContext Context, int leetcodeNumber, string[] userIdsWithSubmissions)
        {
            string leetcodeName = Context.Channel.Name;
            leetcodeName = leetcodeName.Substring(leetcodeName.IndexOf(' '));

            // Grant permissions - this will give everyone in the challenge thread the following permissions
            await PermissionsService.GrantPermission(Context, Context.Channel.Id.ToString(), Entitlement.SubmitChallenge);
            await PermissionsService.GrantPermission(Context, Context.Channel.Id.ToString(), Entitlement.VerifySubmission);
            await PermissionsService.GrantPermission(Context, Context.Channel.Id.ToString(), Entitlement.ViewChallengeSubmissions);
            await PermissionsService.GrantPermission(Context, Context.Channel.Id.ToString(), Entitlement.SubscribeShameTrain);
            await PermissionsService.GrantPermission(Context, Context.Channel.Id.ToString(), Entitlement.UnsubscribeShameTrain);

            // Store challenge details in db
            ChallengeModel challenge = new ChallengeModel()
            {
                ChallengeId = Context.Channel.Id.ToString(),
                ServerId = Context.Guild.Id.ToString(),
                CreationDate = Context.Channel.CreatedAt.DateTime,
                LeetcodeName = leetcodeName,
                LeetcodeNumber = leetcodeNumber
            };
            await Program._challengeRepo.AddChallenge(challenge);

            foreach(string userId in userIdsWithSubmissions)
            {
                await SubmitSolution(Context, userId, null, Language.Unknown, TimeComplexity.Unknown);
            }
        }

        public static async Task CreateDailyChallenge(SocketInteractionContext Context, string leetcodeURL, string leetcodeName, int leetcodeNumber)
        {
            // Create new thread
            var channel = Context.Guild.GetChannel(Context.Channel.Id) as ITextChannel;
            string date = $"{DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.Year}";
            var newThread = await channel.CreateThreadAsync(
                name: $"{date} {leetcodeName}",
                autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                invitable: true,
                type: ThreadType.PublicThread
            );
            await newThread.SendMessageAsync(leetcodeURL);
            await newThread.SendMessageAsync($"{date},{leetcodeNumber},{leetcodeName}");

            // Tag all subscribed users in challenge thread
            List<ChallengeSubscriberModel> subscribed = await Program._challengeSubscriberRepo.GetSubscribersByDiscordId(Context.Guild.Id.ToString());
            List<string> userMentions = new List<string>();
            foreach (ChallengeSubscriberModel user in subscribed)
            {
                userMentions.Add($"<@{user.UserId}>");
            }
            await newThread.SendMessageAsync($"{string.Join(",", userMentions)}");

            // Grant permissions - this will give everyone in the challenge thread the following permissions
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.SubmitChallenge);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.VerifySubmission);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.ViewChallengeSubmissions);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.SubscribeShameTrain);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.UnsubscribeShameTrain);

            // Store challenge details in db
            ChallengeModel challenge = new ChallengeModel()
            {
                ChallengeId = newThread.Id.ToString(),
                ServerId = channel.GuildId.ToString(),
                CreationDate = DateTime.Now,
                LeetcodeName = leetcodeName,
                LeetcodeNumber = leetcodeNumber
            };
            await Program._challengeRepo.AddChallenge(challenge);

            // Copy over currently subscibed users
            // This is to maintain a record of who participated in what challenges even after someone unsubscribes
            foreach (ChallengeSubscriberModel user in subscribed)
            {
                await Program._challengeSubscriberRepo.AddSubscriber(newThread.Id.ToString(), user.UserId, DateTime.Now);
            }
        }

        public static async Task<bool> VerifySubmission(ulong challengeId, ulong userId)
        {
            DirectoryInfo taskDirectory = new DirectoryInfo($"{Constants.ShameTrainChallengeDirectory}{challengeId}{Constants.slash}");
            FileInfo[] taskFiles = taskDirectory.GetFiles($"{userId}*");
            return taskFiles.Length > 0;
        }

        public static async Task<string[]> SubmitSolution(SocketInteractionContext Context, Attachment solution, Language language, TimeComplexity timeComplexity)
        {
            return await SubmitSolution(Context, Context.User.Id.ToString(), solution, language, timeComplexity);
        }

        public static async Task<string[]> SubmitSolution(SocketInteractionContext Context, string UserId, Attachment solution, Language language, TimeComplexity timeComplexity)
        {
            await SubscribeUser(Context, Context.Channel.Id.ToString(), Context.User.Id.ToString());
            await SubscribeUser(Context, Context.Guild.Id.ToString(), Context.User.Id.ToString());
            List<string> lines = new List<string>();
            if (solution != null)
            {
                using (var reader = new StreamReader(await HttpUtilities.DownloadFileToStream(Program._httpClient, solution.Url)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            await Program._challengeSubmissionRepo.AddSubmission(Context.Channel.Id.ToString(), Context.User.Id.ToString(), timeComplexity, language, DateTime.Now);
            return lines.ToArray();
        }

        public static async Task SubscribeUser(SocketInteractionContext Context, string discordId, string userId)
        {
            await Program._challengeSubscriberRepo.AddSubscriber(discordId, userId, DateTime.Now);
        }

        public static async Task UnsubscribeUser(SocketInteractionContext Context, string discordId, string userId)
        {
            await Program._challengeSubscriberRepo.RemoveSubscriber(discordId, userId);

        }
    }
}
