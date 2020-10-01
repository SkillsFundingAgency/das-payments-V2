﻿using System;
﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.Core
{
    public static class Extensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static Dictionary<string, string> ConcatDictionary(this Dictionary<string, string> first,
            Dictionary<string, string> second)
        {
            return second == null ? first : first.Concat(second).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static decimal AsRounded(this decimal unrounded)
        {
            return Math.Round(unrounded, 5);
        }

        public static decimal? AsRounded(this decimal? unrounded)
        {
            return unrounded.HasValue ? AsRounded((decimal?) unrounded.Value) : default(decimal?);
        }

        public static byte GetPeriodFromDate(this DateTime date)
        {
            byte period;
            var month = date.Month;

            if (month < 8)
            {
                period = (byte) (month + 5);
            }
            else
            {
                period = (byte) (month - 7);
            }
            return period;
        }
    }
}


