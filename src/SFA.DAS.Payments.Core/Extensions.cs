using System.Collections.Generic;
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
    }
}