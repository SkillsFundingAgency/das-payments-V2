using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
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

            if (duration.Contains("months"))
            {
                var months = int.Parse(duration.Replace("months", string.Empty));
                return startDate.ToDate().AddMonths(months) - startDate.ToDate();
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
    }
}