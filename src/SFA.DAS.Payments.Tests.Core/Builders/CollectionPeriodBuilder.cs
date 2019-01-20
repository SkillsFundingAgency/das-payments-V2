using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class CollectionPeriodBuilder 
    {
        private byte Period { get; set; } = byte.MaxValue;
        protected int Year { get; set; } = -1;
        protected int Month { get; set; } = -1;

        public CollectionPeriod Build()
        {
            short academicYear;

            if (Month < 8)
            {
                academicYear = (short)((Year - 2000 - 1) * 100 + (Year - 2000));
            }
            else
            {
                academicYear = (short)((Year - 2000) * 100 + (Year - 2000 + 1));
            }

            if (Month < 8)
            {
                Period = (byte)(Month + 5);
            }
            else
            {
                Period = (byte)(Month - 7);
            }

            var name = $"{academicYear}-R{Period:D2}";

            var instance = new CollectionPeriod
            {
                AcademicYear = academicYear,
                Period = Period,
            };
            return instance;
        }

        private void ProcessSpecDate(string date)
        {
            var dateTime = date.ToDate();
            Year = dateTime.Year;
            Month = dateTime.Month;
        }

        public CollectionPeriodBuilder WithSpecDate(string date)
        {
            if (date.StartsWith("R13") || date.StartsWith("R14"))
            {
                var modifiedDate = date.Replace("R13", "R12").Replace("R14", "R12");
                ProcessSpecDate(modifiedDate);
            }
            else
            {
                ProcessSpecDate(date);
            }

            Period = byte.Parse(date.Substring(1, 2));

            return this;
        }

        public CollectionPeriodBuilder WithDate(DateTime date)
        {
            Year = date.Year;
            Month = date.Month;

            return this;
        }
    }
}
