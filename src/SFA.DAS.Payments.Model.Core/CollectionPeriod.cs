using System;
using System.Diagnostics;

namespace SFA.DAS.Payments.Model.Core
{
    [DebuggerDisplay("Collection: {Name}")]
    public class CollectionPeriod : SfaPeriodBaseClass
    {
        public string AcademicYear { get; set; }
        public string Name { get; set; }

        public static CollectionPeriod CreateFromAcademicYearAndPeriod(string academicYear, byte period)
        {
            if (string.IsNullOrEmpty(academicYear) || academicYear.Length != 4)
            {
                throw new ArgumentNullException(nameof(academicYear), "Please ensure that the academic year is of format '1819'");
            }

            var result = new CollectionPeriod();
            if (period < 6)
            {
                result.Month = (byte)(period + 7);
                if (int.TryParse(academicYear.Substring(0,2), out var year)) result.Year = (short)(2000 + year);
            }
            else
            {
                result.Month = (byte)(period - 5);
                if (int.TryParse(academicYear.Substring(2), out var year)) result.Year = (short)(2000 + year);
            }

            if (period > 12)
            {
                result.Month++;
            }

            result.Period = period;
            result.AcademicYear = academicYear;
            result.Name = $"{academicYear}-R{period:D2}";

            return result;
        }

        public CollectionPeriod Clone()
        {
            return (CollectionPeriod) MemberwiseClone();
        }
    }
}