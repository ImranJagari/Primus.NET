using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Primus.NET.Extensions
{
    public static class TimeExtensions
    {
        public static int GetUnixTimeStamp(this DateTime date)
        {
            return (int)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }
    }
}
