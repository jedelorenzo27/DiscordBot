namespace BotShared.models
{
    public class ChallengeModel
    {
        public enum LeetcodeDifficulty
        {
            Easy,
            Medium,
            Hard
        }


        public string ChallengeId { get; set; } // the discord thread and this challenge's unique id
        public string ChannelId { get; set; } // the channel this challenge was posted in
        public string ServerId { get; set; } // the Discord server this challenge belongs to
        public DateTime CreationDate { get; set; }
        public string LeetcodeName { get; set; }
        public int LeetcodeNumber { get; set; }
        public LeetcodeDifficulty ChallengeDifficulty { get; set; }
    }
}
