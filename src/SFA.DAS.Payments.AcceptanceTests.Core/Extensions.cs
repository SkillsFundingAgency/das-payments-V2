using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public static class Extensions
    {
        public static CalendarPeriod ToCalendarPeriod(this DateTime date)
        {
            return new CalendarPeriod((short)date.Year, (byte)date.Month);
        }
            
        public static DateTime ToDate(this string dateText)
        {
            var parts = dateText.Split('/');
            if (parts.Length == 3)
            {
                var monthAndYear = ParseTwoPartDescription($"{parts[1]}/{parts[2]}");
                try
                {
                    var result = monthAndYear.AddDays(int.Parse(parts[0]) - 1);
                    return result;
                }
                catch (Exception e)
                {
                    throw new Exception("Please use a format like: 03/Aug/current academic year", e);
                }
            }
            return ParseTwoPartDescription(dateText);
        }

        private static DateTime ParseTwoPartDescription(string dateText)
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
                    var parts = dateText.Split('/');
                    if (parts.Length != 2)
                        Assert.Fail(
                            $"Invalid period format found.  Expected month or period followed by year text e.g. R01/current Academic Year or Aug/Last Academic Year but found: {dateText}");
                    var period = parts[0].StartsWith("R", StringComparison.OrdinalIgnoreCase)
                        ? int.Parse(parts[0].ToLower().Replace("r", ""))
                        : parts[0].ToMonthPeriod();

                    var date = new DateTime(DateTime.Today.Month >= 8 ? DateTime.Today.Year : DateTime.Today.Year - 1, 8, 1);
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

        public static CalendarPeriod ToCalendarPeriod(this string periodText)
        {
            if (periodText == "start of academic year") 
                periodText = "Aug/Current Academic Year";

            var bits = periodText.Split('/');
            var monthName = bits[0];
            var yearName = bits[1];
            byte period;

            if (DateTime.TryParseExact(monthName, "MMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
            {
                period = (byte)(date.Month > 7 ? date.Month - 7 : date.Month + 5);
            }
            else
            {
                period = byte.Parse(monthName.Replace("R", null));
            }

            return new CalendarPeriod(yearName.ToAcademicYear(), period);
        }

        public static string GetCollectionYear(this CalendarPeriod calendarPeriod)
        {
            return calendarPeriod.Name.Split('-').FirstOrDefault();
        }

        public static decimal ToPercent(this string stringPercent)
        {
            return decimal.Parse(stringPercent.TrimEnd('%')) / 100m;
        }
    }
}