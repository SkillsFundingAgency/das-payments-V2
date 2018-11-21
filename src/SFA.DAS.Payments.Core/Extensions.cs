using System;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.Core
{
    public static class Extensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static decimal AsRounded(this decimal unrounded)
        {
            return Math.Round(unrounded, 5);
        }

        public static decimal? AsRounded(this decimal? unrounded)
        {
            return unrounded.HasValue ? AsRounded((decimal?) unrounded.Value) : default(decimal?);
        }
    }
}