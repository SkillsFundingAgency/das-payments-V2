using System;
using System.Diagnostics;

namespace SFA.DAS.Payments.Model.Core
{
    [DebuggerDisplay("Collection: {Name}")]
    public class CollectionPeriod 
    {
        public string AcademicYear { get; set; }
        public string Name { get; set; }
        public byte Period { get; set; }

        public static CollectionPeriod CreateFromAcademicYearAndPeriod(string academicYear, byte period)
        {
            if (string.IsNullOrEmpty(academicYear) || academicYear.Length != 4)
            {
                throw new ArgumentNullException(nameof(academicYear), "Please ensure that the academic year is of format '1819'");
            }

            var result = new CollectionPeriod();

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