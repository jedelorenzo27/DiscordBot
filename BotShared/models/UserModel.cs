using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotShared.models
{
    public enum UserType
    {
        User,
        Server,
        Channel,
        Thread
    }

    public class UserModel
    {
        public string UserId { get; set; }
        public UserType UserType { get; set; }
        public string Name { get; set; }
        public bool Banned { get; set; }
    }
}
