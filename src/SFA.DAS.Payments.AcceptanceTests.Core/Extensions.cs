using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public static class Extensions
    {
        //public static CalendarPeriod ToLastOnProgPeriod(this DateTime date)
        //{
        //    var lastDayOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
        //    if (date.Day < lastDayOfMonth)
        //    {
        //        var newDate = date.AddMonths(-1);
        //        return newDate.ToCalendarPeriod();
        //    }

        //    return date.ToCalendarPeriod();
        //}

        //public static CalendarPeriod ToCalendarPeriod(this DateTime date)
        //{
        //    return new CalendarPeriod((short)date.Year, (byte)date.Month);
        //}

        //public static CalendarPeriod ToCalendarPeriod(this string periodText)
        //{
        //    if (periodText == "start of academic year")
        //        periodText = "Aug/Current Academic Year";

        //    var bits = periodText.Split('/');
        //    var monthName = (bits.Length > 2) ? bits[1] : bits[0];
        //    var yearName = (bits.Length > 2) ? bits[2] : bits[1];
        //    byte period;

        //    if (DateTime.TryParseExact(monthName, "MMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
        //    {
        //        period = (byte)(date.Month > 7 ? date.Month - 7 : date.Month + 5);
        //    }
        //    else
        //    {
        //        period = byte.Parse(monthName.Replace("R", null));
        //    }

        //    return new CalendarPeriod(yearName.ToAcademicYear(), period);
        //}

        //public static string GetCollectionYear(this CalendarPeriod calendarPeriod)
        //{
        //    return calendarPeriod.AcademicYear;
        //}

        public static decimal ToPercent(this string stringPercent)
        {
            if (decimal.TryParse(stringPercent.TrimEnd('%'), out var result))
            {
                return result / 100;
            }
            throw new Exception("Please include the SFA Contribution % in the price episodes or earnings");
        }

        private static ConcurrentDictionary<int, PropertyInfo> periodisedProperties;

        private static ConcurrentDictionary<int, PropertyInfo> PeriodisedProperties
        {
            get
            {
                if (periodisedProperties == null)
                {
                    periodisedProperties = new ConcurrentDictionary<int, PropertyInfo>();

                    for (var i = 1; i < 13; i++)
                    {
                        periodisedProperties.TryAdd(i, typeof(PriceEpisodePeriodisedValues).GetProperty("Period" + i));
                    }
                }

                return periodisedProperties;
            }
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
            if (!PeriodisedProperties.TryGetValue(period, out var propertyInfo))
                throw new KeyNotFoundException($"There is no Periodised Property Info found for Period {period}");

            return propertyInfo;
        }
    }
}