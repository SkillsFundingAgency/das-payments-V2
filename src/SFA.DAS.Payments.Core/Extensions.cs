using System;
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

        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (a, b) => (a, b))
                .SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => resultSelector(x.a, b));
        }
    }
}


