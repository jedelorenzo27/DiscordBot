using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utilities
{
    public class Version
    {
        public int Major;
        public int Minor;
        public int Patch;

        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor; 
            Patch = patch;
        }

        public Version(string version)
        {
            string[] parts = version.Split('.');
            Major = int.Parse(parts[0]);
            Minor = int.Parse(parts[1]);
            Patch = int.Parse(parts[2]);
        }

        public bool IsNewer(Version version)
        {
            if (Major > version.Major)
            {
                return true;
            } else if (Minor > version.Minor)
            {
                return true;
            } else
            {
                return Patch > version.Patch;
            }
        }

        override
        public string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}
