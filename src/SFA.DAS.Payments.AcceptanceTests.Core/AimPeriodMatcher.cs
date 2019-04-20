using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public static class AimPeriodMatcher
    {
        public static bool IsStartDateValidForCollectionPeriod(
            string startDate,
            CollectionPeriod collectionPeriod,
            TimeSpan? plannedDurationAsTimeSpan,
            TimeSpan? actualDurationAsTimeSpan,
            CompletionStatus completionStatus,
            string aimReference,
            string plannedDuration,
            string actualDuration)
        {
            return true;
        }

        public static CollectionPeriod GetEndPeriodForAim(CollectionPeriod aimStartDate, string duration)
        {
            var startPeriod = aimStartDate.Period;

            if (duration.Contains("months"))
            {
                var months = int.Parse(duration.Replace("months", string.Empty));

                return new CollectionPeriod
                { AcademicYear = aimStartDate.AcademicYear, Period = (byte)(startPeriod + months - 1) };
            }

            return aimStartDate;
        }

        public static bool FinishesBefore(this CollectionPeriod currentPeriod, CollectionPeriod otherPeriod)
        {
            if (otherPeriod.AcademicYear < currentPeriod.AcademicYear)
            {
                return true;
            }

            if (otherPeriod.AcademicYear > currentPeriod.AcademicYear)
            {
                return false;
            }

            return otherPeriod.Period > currentPeriod.Period;
        }
    }
}