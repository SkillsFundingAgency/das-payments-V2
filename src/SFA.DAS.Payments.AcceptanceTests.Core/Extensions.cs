using System;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public static class Extensions
    {
        public static DateTime ToDate(this string dateText)
        {
            switch (dateText.ToLower())
            {
                case "today":
                    return DateTime.Today;
                case "yesterday":
                    return DateTime.Today.AddDays(-1);
                case "tomorrow":
                    return DateTime.Today.AddDays(1);
                case "last year":
                    return DateTime.Today.AddYears(-1);
                case "next year":
                    return DateTime.Today.AddYears(1);
                case "start of academic year":
                    return new DateTime(DateTime.Today.Month >= 8 ? DateTime.Today.Year : DateTime.Today.Year - 1, 8, 1);
                case "start of last academic year":
                    return new DateTime(DateTime.Today.Month >= 8 ? DateTime.Today.Year - 1 : DateTime.Today.Year - 2, 8, 1);
                default:
                    if (!dateText.StartsWith("R", StringComparison.OrdinalIgnoreCase))
                        return DateTime.Parse(dateText);
                    var today = DateTime.Today;
                    var period = int.Parse(dateText.ToLower().Replace("r", ""));
                    var month = period >= 5 ? period - 4 : period + 8;
                    if (month >= 8 && period >= 6)
                        return new DateTime(today.Year + 1, month, 1);
                    if (month <= 7 && period <= 5)
                        return new DateTime(today.Year - 1, month, 1);
                    return new DateTime(today.Year, month, 1);
            }
        }

        public static int ToMonth(this byte period)
        {
            return (period >= 5 ? period - 4 : period + 8);
        }

        public static short ToYear(this byte period, string collectionYear)
        {
            var part = collectionYear.Substring(period < 5 ? 0 : 2, 2);
            return (short)(short.Parse(part) + 2000);
        }
    }
}