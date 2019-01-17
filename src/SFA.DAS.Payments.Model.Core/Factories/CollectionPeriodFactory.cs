using System;

namespace SFA.DAS.Payments.Model.Core.Factories
{
    public static class CollectionPeriodFactory
    {
        public static CollectionPeriod CreateFromAcademicYearAndPeriod(short academicYear, byte period)
        {
            if (academicYear < 1600 || academicYear > 2200)
            {
                throw new ArgumentNullException(nameof(academicYear), "Please ensure that the academic year is of format '1819'");
            }

            var result = new CollectionPeriod();

            result.Period = period;
            result.AcademicYear = academicYear;
            result.Name = $"{academicYear}-R{period:D2}";

            return result;
        }
    }
}
