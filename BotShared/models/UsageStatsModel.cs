using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public enum StatTypeSuffix
    {
        Success,
        Failure
    }

    public class UsageStatsModel
    {
        public string Id { get; set; }
        public string StatType { get; set; }
        public int TimesUsed { get; set; }
    }
}
