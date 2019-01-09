using System;
using System.Diagnostics;

namespace SFA.DAS.Payments.Model.Core
{
    [DebuggerDisplay("Delivery: {Identifier}")]
    public class DeliveryPeriod : SfaPeriodBaseClass
    {
        public DeliveryPeriod Clone()
        {
            return (DeliveryPeriod) MemberwiseClone();
        }

        public string Identifier { get; set; }

        public static DeliveryPeriod CreateFromAcademicYearAndPeriod(string academicYear, byte period)
        {
            if (string.IsNullOrEmpty(academicYear) || academicYear.Length != 4)
            {
                throw new ArgumentNullException(nameof(academicYear), "Please ensure that the academic year is of format '1819'");
            }

            var result = new DeliveryPeriod();
            if (period < 6)
            {
                result.Month = (byte)(period + 7);
                if (int.TryParse(academicYear.Substring(0, 2), out var year)) result.Year = (short)(2000 + year);
            }
            else
            {
                result.Month = (byte)(period - 5);
                if (int.TryParse(academicYear.Substring(2), out var year)) result.Year = (short)(2000 + year);
            }

            result.Period = period;
            result.Identifier = $"{result.Year}-{result.Month:D2}";

            return result;
        }
    }
}