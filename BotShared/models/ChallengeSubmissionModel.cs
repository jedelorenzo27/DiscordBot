﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public class ChallengeSubmissionModel
    {
        public string ChannelId { get; set; }
        public string UserId { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}
