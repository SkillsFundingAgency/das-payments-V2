using System;
using System.Configuration;
using System.Globalization;

namespace SFA.DAS.Payments.Tests.Core
{
    public static class SpecDateExtensions
    {
        public static DateTime ToDate(this string dateText)
        {
            var parts = dateText.Split('/');
            if (parts.Length == 3)
            {
                var monthAndYear = ParseTwoPartDescription($"{parts[1]}/{parts[2]}");
                try
                {
                    return monthAndYear.AddDays(int.Parse(parts[0]) - 1);
                }
                catch (Exception e)
                {
                    throw new Exception("Please use a format like: 03/Aug/current academic year", e);
                }
            }
            return ParseTwoPartDescription(dateText);
        }

        public static DateTime? ToNullableDate(this string dateText)
        {
            return string.IsNullOrWhiteSpace(dateText) ? default(DateTime?) : dateText.ToDate();
        }
        private static DateTime ParseTwoPartDescription(string dateText)
        {
            string GetAppSetting(string keyName)
            {
                return ConfigurationManager.AppSettings[keyName] ?? throw new InvalidOperationException($"{keyName} not found in app settings.");
            }

            var enableRollOverTesting = bool.Parse(GetAppSetting("EnableRollOverTesting") ?? "false");

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
                    var parts = dateText.Split('/');
                    if (parts.Length != 2)
                        throw new Exception($"Invalid period format found.  Expected month or period followed by year text e.g. R01/current Academic Year or Aug/Last Academic Year but found: {dateText}");
                    int period;
                    var returnPeriodPart = parts[0];
                    if (returnPeriodPart.StartsWith("R", StringComparison.OrdinalIgnoreCase))
                    {
                        var returnPeriod = parts[0].ToLower().Replace("r", "");
                        period = int.Parse(returnPeriod);
                    }
                    else
                    {
                        period = parts[0].ToMonthPeriod();
                    }

                    var year = (DateTime.Today.Month >= 8 ? DateTime.Today.Year : DateTime.Today.Year - 1);

                    if (enableRollOverTesting) year += 1;

                    var date = new DateTime(year, 8, 1);

                    date = date.AddMonths(period - 1);

                    var yearText = parts[1].ToLower();

                    if (yearText.Equals("last academic year"))
                        date = date.AddYears(-1);
                    else if (yearText.Equals("next academic year"))
                        date = date.AddYears(1);
                    return date;
            }
        }

        public static int ToMonthNumber(this string month)
        {
            return DateTime.ParseExact(month.ToLower().Substring(0, 3), "MMM", CultureInfo.CurrentCulture).Month;
        }

        public static int ToMonthPeriod(this string month)
        {
            switch (month.ToLower().Substring(0, 3))
            {
                case "jan":
                    return 6;
                case "feb":
                    return 7;
                case "mar":
                    return 8;
                case "apr":
                    return 9;
                case "may":
                    return 10;
                case "jun":
                    return 11;
                case "jul":
                    return 12;
                case "aug":
                    return 1;
                case "sep":
                    return 2;
                case "oct":
                    return 3;
                case "nov":
                    return 4;
                case "dec":
                    return 5;
                default:
                    throw new InvalidOperationException($"Invalid month: {month}");
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

        public static string ToPeriodText(this byte period)
        {
            return $"R{period:D2}";
        }

        public static string ToAcademicYear(this string yearName)
        {
            int year;
            switch (yearName)
            {
                case "Current Academic Year":
                    year = DateTime.Today.Month < 8 ? DateTime.Today.Year - 1 : DateTime.Today.Year;
                    break;
                case "Last Academic Year":
                    year = DateTime.Today.Month < 8 ? DateTime.Today.Year - 2 : DateTime.Today.Year - 1;
                    break;
                default:
                    throw new NotImplementedException("if it was meant to be anything other than Current/Last Academic Year, it needs to be implemented");
            }
            return string.Concat(year - 2000, year - 1999);
        }

    }
}
