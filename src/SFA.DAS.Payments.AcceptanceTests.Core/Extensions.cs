using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;

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

        public static CalendarPeriod ToLastOnProgPeriod(this DateTime date)
        {
            var lastDayOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
            if (date.Day < lastDayOfMonth)
            {
                var newDate = date.AddMonths(-1);
                return newDate.ToCalendarPeriod();
            }

            return date.ToCalendarPeriod();
        }

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
                    return monthAndYear.AddDays(int.Parse(parts[0]) - 1);
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
            var monthName = (bits.Length > 2) ? bits[1] : bits[0];
            var yearName = (bits.Length > 2) ? bits[2] : bits[1];
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
            return calendarPeriod.AcademicYear;
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