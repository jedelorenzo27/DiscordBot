﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public class ChallengeSubsciberModel
    {
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public DateTime SubscribeDate { get; set; }
    }
}
