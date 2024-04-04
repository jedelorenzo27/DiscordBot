using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public class ChallengeSubscriberModel
    {
        public string ChannelId { get; set; }
        public string UserId { get; set; }
        public DateTime SubscribeDate { get; set; }
    }
}
