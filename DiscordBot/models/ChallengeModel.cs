using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.models
{
    public class ChallengeModel
    {
        public ulong ServerId;
        public ulong ChannelId;
        public DateTime CreationDate;
        public string LeetcodeName; 
        public int LeetcodeId;
    }
}
