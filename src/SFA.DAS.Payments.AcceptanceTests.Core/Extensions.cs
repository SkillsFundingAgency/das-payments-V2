using System;
using System.Globalization;
using SFA.DAS.Payments.Model.Core;

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

        public static short ToYear(this string yearName)
        {
            short year;
            if (yearName == "Current Academic Year")
                year = (short) DateTime.Today.Year;
            else
                throw new NotImplementedException("if it was meant to be anything other than Current Academic Year, it needs to be implemented");
            return year;
        }

        public static CalendarPeriod ToCalendarPeriod(this string periodText)
        {
            short year;
            byte month;
            var bits = periodText.Split('/');
            var monthName = bits[0];
            var yearName = bits[1];

            year = ToYear(yearName);

            month = (byte)DateTime.ParseExact(bits[0], "MMM", CultureInfo.CurrentCulture).Month;

            return new CalendarPeriod(year, month);
        }
    }
}