using Discord;
using Discord.Interactions;
using SpecterAI.services;
using DiscordBot.Utilities;
using SpecterAI.Utilities;

namespace DiscordBot.services
{

    public class ChallengeDetails
    {
        public string ChallengeName = "";
        public int ChallengeId = -1;
        public string ChallengeDate = "";
    }

    public static class ShameTrainServices
    {
        

        public static async Task JumpStartShameTrain()
        {
            SubscribedUsers = await ShameTrainFileLoader.LoadSubscribedUsers();

            if (!Directory.Exists(Constants.ShameTrainChallengeDirectory))
            {
                Directory.CreateDirectory(Constants.ShameTrainChallengeDirectory);
            }

        }

        public static async Task Shame()
        {

        }

        public static async Task CreateDailyChallenge(SocketInteractionContext Context, string leetcodeURL, string challengeName, string challengeId)
        {

            // Create new thread
            var channel = Context.Guild.GetChannel(Context.Channel.Id) as ITextChannel;
            string date = $"{DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.Year}";
            var newThread = await channel.CreateThreadAsync(
                name: $"{date} {challengeName}",
                autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                invitable: true,
                type: ThreadType.PublicThread
            );
            await newThread.SendMessageAsync(leetcodeURL);
            await newThread.SendMessageAsync($"{date},{challengeId},{challengeName}");
            if (Context.Channel.Id == Constants.ByteLordzCringeCave_Channel_The_Daily)
            {
                await newThread.SendMessageAsync($"{MentionUtils.MentionRole(Constants.ByteLordzCringeCave_Role_Unemployed)}{MentionUtils.MentionRole(Constants.ByteLordzCringeCave_Role_Everyone)}");
            }

            // Grant permissions - this will give everyone in the challenge thread the following permissions
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.SubmitChallenge);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.VerifySubmission);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.ViewChallengeSubmissions);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.SubscribeShameTrain);
            await PermissionsService.GrantPermission(Context, newThread.Id.ToString(), Entitlement.UnsubscribeShameTrain);

            // Create challenge folder
            // Files are saved under thread id
            // output/shame_train/challange/<thread_id>/
            //                                         /challangeDetails.txt
            //                                         /subscribed_users.txt
            //                                         /ect ...
            Console.WriteLine($"Creating new directory here: {Constants.ShameTrainChallengeDirectory}{newThread.Id}{Constants.slash}");
            string challange_directory = $"{Constants.ShameTrainChallengeDirectory}{newThread.Id}{Constants.slash}";
            
            Directory.CreateDirectory(challange_directory);
            Console.WriteLine("Done creating directories");

            // Copy over currently subscibed users
            // This is to maintain a record of who participated in what challenges even after someone unsubscribes
            ShameTrainFileLoader.CopySubscribedUsersToChallengeFolder(challange_directory);


            // Save details
            ChallengeDetails details = new ChallengeDetails();
            details.ChallengeName = challengeName;
            details.ChallengeId = int.Parse(challengeId);
            details.ChallengeDate = date;
            ShameTrainFileLoader.SaveChallengeDetails(newThread.Id, details);

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

        public static async Task SubscribeUser(SocketInteractionContext Context, ulong userId)
        {
            HashSet<ulong> subscribed_users = await ShameTrainFileLoader.LoadSubscribedUsers();
            if (!subscribed_users.Contains(userId))
            {
                subscribed_users.Add(userId);
                await ShameTrainFileLoader.SaveSubscribedUsers(subscribed_users);
            }
        }

        public static async Task UnsubscribeUser(SocketInteractionContext Context, ulong userId)
        {
            HashSet<ulong> subscribed_users = await ShameTrainFileLoader.LoadSubscribedUsers();
            subscribed_users.Remove(userId);
            await ShameTrainFileLoader.SaveSubscribedUsers(subscribed_users);
        }


        // Unused until I can figure out how to download leetcode webpage
        private static (string name, string id) ExtractChallengeInfoFromSource(string source, string original_url)
        {
            // HTML Title Example
            //< a class="no-underline hover:text-blue-s dark:hover:text-dark-blue-s truncate cursor-text whitespace-normal hover:!text-[inherit]" href="/problems/two-sum/">1. Two Sum</a>

            // Example url:
            //https://leetcode.com/problems/two-sum/description/
            original_url.Replace("http://", "");

            // Example urlParts:
            // { leetcode.com, problems, two-sum, description }
            string[] urlParts = original_url.Split('/');

            string startSearchString = $"hover:!text-[inherit]\" href =\"/{urlParts[1]}/{urlParts[2]}\">";
            Console.WriteLine($"startSearchString: {startSearchString}");

            int titleStartIndex = source.IndexOf(startSearchString);
            Console.WriteLine($"titleStartIndex: {titleStartIndex}");

            int titleEndIndex = source.IndexOf("</a>", titleStartIndex);
            Console.WriteLine($"titleEndIndex: {titleEndIndex}");

            string titleAndId = source.Substring(titleStartIndex, titleEndIndex - titleStartIndex);


            Console.WriteLine($"titleAndId: {titleAndId}");
            string[] titleParts = titleAndId.Split('.');
            Console.WriteLine($"name: {titleParts[1]}");
            Console.WriteLine($"is: {titleParts[0]}");
            return (titleParts[1].Trim(), titleParts[0].Trim());
        }

        private static async Task<string> ExtractAndFormatProblemNameFromURL(string url)
        {
            //Example url
            //https://leetcode.com/problems/two-sum/description/
            url.Replace("http://", "");

            string[] urlParts = url.Split('/');

            int expectedNumberOfParts = 4;
            if (urlParts.Length != expectedNumberOfParts)
            {
                string[] errorMessages = new string[]
                {
                    $"URL had a suprising number of parts. Expected:{expectedNumberOfParts}, Recieved:{urlParts.Length}",
                    $"URL in question: {url}"
                };
                await LoggingService.LogMessage(LogLevel.Error, errorMessages);
            }

            string rawName = urlParts[2];


            return "";
        }

    }

    public static class ShameTrainFileLoader
    {

        public static async Task<HashSet<ulong>> LoadSubscribedUsers()
        {
            try
            {
                if (File.Exists(Constants.ShameTrainSubscribedUsersFilePath))
                {
                    HashSet<ulong> userIds = new HashSet<ulong>();
                    string[] string_userIds = File.ReadAllText(Constants.ShameTrainSubscribedUsersFilePath).Split(Constants.ShameTrainSubscribedUsersFileDelimiter);
                    foreach (string userId in string_userIds)
                    {
                        userIds.Add(ulong.Parse(userId));
                    }
                }
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
                File.WriteAllText(Constants.ShameTrainSubscribedUsersFilePath, string.Join(Constants.ShameTrainSubscribedUsersFileDelimiter, subscribedUsers));
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
                string fullFilePath = $"{Constants.ShameTrainChallengeDirectory}{challangeId}{Constants.slash}{fileName}";
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
            string fullFilePath = $"{Constants.ShameTrainChallengeDirectory}{challangeId}{Constants.slash}{fileName}";
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            await HttpUtilities.DownloadFileAsync(Program._httpClient, solution.Url, fullFilePath);
        }


        public static ChallengeDetails LoadChallengeDetails(ulong challengeId)
        {
            ChallengeDetails challengeDetails = new ChallengeDetails();
            string challengeDirectory = Constants.ShameTrainChallengeDirectory + challengeId;
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
            File.WriteAllLines($"{Constants.ShameTrainChallengeDirectory}{challengeId}{Constants.slash}{Constants.ShameTrainChallengeDetailsFileName}", lines.ToArray());
        }

    }
}
