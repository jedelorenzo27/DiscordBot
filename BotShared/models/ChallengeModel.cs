namespace BotShared.models
{
    public class ChallengeModel
    {
        public long ServerId { get; set; }
        public long ChannelId { get; set; }
        public DateTime CreationDate { get; set; }
        public string LeetcodeName { get; set; }
        public int LeetcodeId { get; set; }
    }
}
