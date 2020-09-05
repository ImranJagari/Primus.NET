using PrimusNetServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimusNetServer.Extensions
{
    public static class EnumExtensions
    {
        public static string GetWSValue(this EngineIOMessageTypeEnum value)
        {
            return ((int)value).ToString();
        }
    }
}
