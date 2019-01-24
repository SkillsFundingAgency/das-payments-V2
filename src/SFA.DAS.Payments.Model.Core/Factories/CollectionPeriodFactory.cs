using System;

namespace SFA.DAS.Payments.Model.Core.Factories
{
    public static class CollectionPeriodFactory
    {
        public static CollectionPeriod CreateFromAcademicYearAndPeriod(string academicYear, byte period)
        {
            return CreateFromAcademicYearAndPeriod(short.Parse(academicYear), period);
        }

        public static CollectionPeriod CreateFromAcademicYearAndPeriod(short academicYear, byte period)
        {
            if (academicYear < 1600 || academicYear > 2200)
            {
                throw new ArgumentException(nameof(academicYear), $"Invalid academic year: '{academicYear}'.  Please ensure that the academic year is in the correct format e.g. '1819'");
            }

            if (period < 1 || period > 14)
                throw new ArgumentException(nameof(period), $"Invalid period: '{period}'.");

            return new CollectionPeriod { Period = period, AcademicYear = academicYear };
        }
    }
}
