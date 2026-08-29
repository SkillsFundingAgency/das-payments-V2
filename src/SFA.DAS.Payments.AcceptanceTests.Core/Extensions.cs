using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public static class Extensions
    {
        private static readonly ConcurrentDictionary<int, PropertyInfo> periodisedProperties;

        static Extensions()
        {
            periodisedProperties = new ConcurrentDictionary<int, PropertyInfo>();

            for (var i = 1; i < 13; i++)
            {
                periodisedProperties.TryAdd(i, typeof(PriceEpisodePeriodisedValues).GetProperty("Period" + i));
            }
        }

        
        public static decimal ToPercent(this string stringPercent)
        {
            if (decimal.TryParse(stringPercent.TrimEnd('%'), out var result))
            {
                return result / 100;
            }
            throw new Exception("Please include the SFA Contribution % in the price episodes or earnings");
        }



        public static decimal? GetValue(this PriceEpisodePeriodisedValues values, int period)
        {
            return (decimal?)GetPropertyInfo(period).GetValue(values);
        }

        public static TimeSpan? ToTimeSpan(this string duration, string startDate)
        {
            if (string.IsNullOrEmpty(duration))
            {
                return null;
            }

            var options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);
            var cleanStr = regex.Replace(duration, " ");

            
            var durationElements = cleanStr.Split(' ');

            if (!durationElements.Contains("-") && Array.Exists(durationElements, x=>x.Contains("days")))
            {
                return startDate.ToDate().AddDays(int.Parse(durationElements[0])) - startDate.ToDate();
            }

            if (Array.Exists(durationElements, x => x.Contains("month")))
            {
                if (durationElements.Contains("-") && Array.Exists(durationElements, x => x.Contains("day")))
                {
                    return startDate.ToDate().AddMonths(int.Parse(durationElements[0])).
                               AddDays(int.Parse(durationElements[Array.FindIndex(durationElements, 0, x => x.Contains("-")) + 1]) * -1) - startDate.ToDate();
                }

                return startDate.ToDate().AddMonths(int.Parse(durationElements[0])) - startDate.ToDate();
            }

            throw new Exception($"Could not parse duration: {duration}");
        }

        public static void SetValue(this PriceEpisodePeriodisedValues values, int period, decimal? value)
        {
            GetPropertyInfo(period).SetValue(values, value);
        }

        private static PropertyInfo GetPropertyInfo(int period)
        {
            if (!periodisedProperties.TryGetValue(period, out var propertyInfo))
                throw new KeyNotFoundException($"There is no Periodised Property Info found for Period {period}");

            return propertyInfo;
        }

        public static long? ParseAsNullableLong(this string s)
                => long.TryParse(s, out var result) ? result : (long?)null;
    }
}