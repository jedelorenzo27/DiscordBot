using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public enum Entitlement
    {
        // Currently limited to length of 50 characters
        [Description("Grant Permission")]
        GrantPermission,
        [Description("Remove Permission")]
        RemovePermission,
        [Description("View Permissions")]
        ViewPermissions,
        [Description("View Usage")]
        ViewUsage,
        [Description("Ban Other")]
        BanOther,
        [Description("Pokemon")]
        Pokemon,
        [Description("OpenAI Chat")]
        OpenAiChat,
        [Description("OpenAI Image")]
        OpenAiImage,
        [Description("Shame users for not completing the daily")]
        Shame,
        [Description("Create Challenge")]
        CreateChallenge,
        [Description("Backfill Challenge")]
        BackfillChallenge,
        [Description("Delete Challenge")]
        DeleteChallenge,
        [Description("Submit Challenge")]
        SubmitChallenge,
        [Description("Verify Challenge Submission")]
        VerifySubmission,
        [Description("Subscribe To ShameTrain")]
        SubscribeShameTrain,
        [Description("Subscribe Other To ShameTrain")]
        SubscribeOtherToShameTrain,
        [Description("Unsubscribe From ShameTrain")]
        UnsubscribeShameTrain,
        [Description("View Challenge Submissions")]
        ViewChallengeSubmissions
    }

    public class EntitlementModel
    {
        public string UserId { get; set; }
        public Entitlement Entitlement { get; set; }
        public DateTime Granted { get; set; }

        override
        public string ToString()
        {
            return $"userId:{UserId},Entitlement:{Entitlement},Granted:{Granted}";
        }
    }
}
