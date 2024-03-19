using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.models
{
    public class EntitlementModel
    {
        public ulong UserId;
        public Entitlement Entitlement;
        public DateTime Granted;
    }
}
