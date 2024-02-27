using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.Utilities
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this MessageRole val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType().GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }


        public static string ToDescriptionString(this Entitlement val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType().GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
