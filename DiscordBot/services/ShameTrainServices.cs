using Discord;
using Discord.Interactions;
using SpecterAI.services;
using DiscordBot.Utilities;
using SpecterAI.Utilities;
using Version = DiscordBot.Utilities.Version;
using BotShared.models;

namespace DiscordBot.services
{
    public class ChallengeDetails
    {
        public string ChallengeName = "";
        public int ChallengeId = -1;
        public string ChallengeDate = "";
        public Version version = new Version(1,0,0);
    }

    public static class ShameTrainServices
    {
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
            List<ChallengeSubscriberModel> subscribed = await Program._challengeSubscriberRepo.GetSubscribersForChannel(channel.Id.ToString());
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
                ChannelId = channel.Id.ToString(),
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

        public static async Task<string[]> SubmitSolution(SocketInteractionContext Context, Attachment solution, string language, string timeComplexity, ulong challengeId, ulong userId)
        {
            await ShameTrainFileLoader.SaveSolution(solution, language, challengeId, userId);
            return await ShameTrainFileLoader.LoadSolution(language, challengeId, userId);
        }

        public static async Task SubscribeUser(SocketInteractionContext Context, string userId)
        {
            await Program._challengeSubscriberRepo.AddSubscriber(Context.Channel.Id.ToString(), userId, DateTime.Now);
        }

        public static async Task UnsubscribeUser(SocketInteractionContext Context, string userId)
        {
            await Program._challengeSubscriberRepo.RemoveSubscriber(Context.Channel.Id.ToString(), userId);

        }
    }

    public static class ShameTrainFileLoader
    {
        public static async Task<HashSet<ulong>> LoadSubscribedUsers()
        {
            try
            {
                HashSet<ulong> userIds = new HashSet<ulong>();
                string[] string_userIds = File.ReadAllLines(Constants.ShameTrainSubscribedUsersFilePath);
                foreach (string userId in string_userIds)
                {
                    Console.WriteLine(userId);
                    userIds.Add(ulong.Parse(userId));
                }
                return userIds;
            }
            catch (Exception ex)
            {
                string[] errors = new string[]
                {
                    "Failed while loading ShameTrain subscibed users",
                    ex.ToString()
                };
                await LoggingService.LogMessage(LogLevel.Error, errors);
            } 
            return new HashSet<ulong>();
        }

        public static async Task SaveSubscribedUsers(HashSet<ulong> subscribedUsers)
        {
            try
            {
                File.Delete(Constants.ShameTrainSubscribedUsersFilePath);
                File.WriteAllText(Constants.ShameTrainSubscribedUsersFilePath, string.Join("\n", subscribedUsers));
            } catch (Exception ex)
            {
                string[] errors = new string[]
                {
                    "Failed while saving ShameTrain subscibed users",
                    ex.ToString()
                };
                await LoggingService.LogMessage(LogLevel.Error, errors);
            }
        }

        public static void CopySubscribedUsersToChallengeFolder(string toFolder)
        {
            File.Copy(Constants.ShameTrainSubscribedUsersFilePath, $"{toFolder}{Constants.ShameTrainSubscribedUsersFileName}");
        }

        public static async Task<string[]> LoadSolution(string language, ulong challangeId, ulong userId)
        {
            try
            {
                string fileName = $"{userId}.{language}";
                string fullFilePath = $"{Constants.ShameTrainChallengeDirectory}{challangeId}{Constants.slash}{Constants.ShameTrainChallengeSolutionDirectory}{fileName}";
                return File.ReadAllLines(fullFilePath);
            } catch (Exception ex)
            {
                string[] errors = new string[]
                {
                    $"Something went wrong while trying to load solution for challengeId={challangeId} userId={userId}",
                    ex.Message
                };
                await LoggingService.LogMessage(LogLevel.Error, errors);
            }
            return new string[] { };
        }

        public static async Task SaveSolution(Attachment solution, string language, ulong challangeId, ulong userId)
        {
            string fileName = $"{userId}.{language}";
            /*
            // Add versioning for multiple submissions. Fow now only allow one submission
            DirectoryInfo taskDirectory = new DirectoryInfo($"{Constants.ShameTrainChallengeDirectory}{challangeId}");
            FileInfo[] taskFiles = taskDirectory.GetFiles($"*{fileName}");
            Array.Sort(taskFiles, (a,b) => { return a.Name.CompareTo(b.Name); });
            */
            string fullFileDirectory = $"{Constants.ShameTrainChallengeDirectory}{challangeId}{Constants.slash}{Constants.ShameTrainChallengeSolutionDirectory}";
            string fullFilePath = $"{fullFileDirectory}{fileName}";
            Directory.CreateDirectory(fullFileDirectory);
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            await HttpUtilities.DownloadFileAsync(Program._httpClient, solution.Url, fullFilePath);
        }


        public static ChallengeDetails LoadChallengeDetails(ulong challengeId)
        {
            ChallengeDetails challengeDetails = new ChallengeDetails();
            string challengeDirectory = Constants.ShameTrainChallengeDirectory + challengeId + Constants.ShameTrainChallengeSolutionDirectory;
            if (Directory.Exists(challengeDirectory))
            {
                string[] lines = File.ReadAllLines(challengeDirectory + Constants.slash + Constants.ShameTrainChallengeDetailsFileName);
                foreach(string line in lines)
                {
                    string[] parts = line.Split(Constants.ShameTrainChallengeDetailsFileNameDelimiter);
                    switch(parts[0])
                    {
                        case Constants.ChallengeName:
                            challengeDetails.ChallengeName = parts[1];
                            break;
                        case Constants.ChallengeId:
                            challengeDetails.ChallengeId = int.Parse(parts[1]);
                            break;
                        case Constants.ChallengeDate:
                            challengeDetails.ChallengeDate = parts[1];
                            break;
                        case Constants.ChallengeVersion:
                            challengeDetails.version = new Version(parts[1]);
                            break;
                    }
                }
            }
            return challengeDetails;
        }

        public static void SaveChallengeDetails(ulong challengeId, ChallengeDetails details)
        {
            List<string> lines = new List<string>();
            lines.Add($"{Constants.ChallengeName}{Constants.ShameTrainChallengeDetailsFileNameDelimiter}{details.ChallengeName}");
            lines.Add($"{Constants.ChallengeDate}{Constants.ShameTrainChallengeDetailsFileNameDelimiter}{details.ChallengeDate}");
            lines.Add($"{Constants.ChallengeId}{Constants.ShameTrainChallengeDetailsFileNameDelimiter}{details.ChallengeId}");
            lines.Add($"{Constants.ChallengeId}{Constants.ShameTrainChallengeDetailsFileNameDelimiter}{details.version.ToString()}");
            File.WriteAllLines($"{Constants.ShameTrainChallengeDirectory}{challengeId}{Constants.slash}{Constants.ShameTrainChallengeDetailsFileName}", lines.ToArray());
        }

    }
}
