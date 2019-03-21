using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;

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
            string aimReference)
        {
            if (startDate == null) return true;
            var aimStartDate = startDate.ToDate();
            var aimStartPeriod = new CollectionPeriodBuilder().WithDate(aimStartDate).Build();
            var aimDuration = actualDurationAsTimeSpan ?? plannedDurationAsTimeSpan;
            var collectionPeriodReferenceDate = DateFromCollectionPeriod(collectionPeriod);

            return aimStartPeriod.AcademicYear == collectionPeriod.AcademicYear ||
                   (aimStartDate + aimDuration >= collectionPeriodReferenceDate && completionStatus != CompletionStatus.Completed) ||
                   (aimStartDate + aimDuration >= collectionPeriodReferenceDate && aimReference == "ZPROG001" && completionStatus == CompletionStatus.Completed);
        }

        private static DateTime DateFromCollectionPeriod(CollectionPeriod collectionPeriod)
        {
            var year = collectionPeriod.AcademicYear / 100 + 2000;

            return collectionPeriod.Period < 6 
                ? new DateTime(year, collectionPeriod.Period + 7, 1) 
                : new DateTime(year + 1, collectionPeriod.Period - 5, 1);
        }
    }
}