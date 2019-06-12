using System;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class CalculatePeriodStartAndEndDate : ICalculatePeriodStartAndEndDate
    {
        public (DateTime periodStartDate, DateTime periodEndDate) GetPeriodDate(int period, int academicYear)
        {
            var calendarMonth = (period < 6) ? period + 7 : period - 5;
            var year = (academicYear % 100) + 2000; // the second part of the academic year in yyyy format

            if (period < 6)
            {
                year--;
            }

            var day = DateTime.DaysInMonth(year, calendarMonth);
            var startDate = new DateTime(year, calendarMonth, 1);
            var endDate = new DateTime(year, calendarMonth, day);

            return (startDate, endDate);
        }
    }
}