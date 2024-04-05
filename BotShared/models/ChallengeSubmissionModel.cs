using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public enum Language
    {
        [Description("cs")]
        CSharp,
        [Description("js")]
        Javascript,
        [Description("py")]
        Python,
        [Description("myst")]
        Unknown
    }

    public enum TimeComplexity
    {
        [Description("n!")]
        N_Factorial,
        [Description("2^n")]
        Two_To_The_N,
        [Description("n^2")]
        M_Squared,
        [Description("nlogn")]
        N_Log_N,
        [Description("n")]
        N,
        [Description("logn")]
        Log_N,
        [Description("1")]
        Constant,
        [Description("??")]
        Unknown
    }

    public class ChallengeSubmissionModel
    {
        public string ChannelId { get; set; }
        public string UserId { get; set; }
        public TimeComplexity BigO {  get; set; }
        public Language ProgrammingLanguage { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}
