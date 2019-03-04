using System;
using System.Collections.Generic;
using System.Linq;
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
            var aimStartDate = startDate.ToDate();
            var aimStartPeriod = new CollectionPeriodBuilder().WithDate(aimStartDate).Build();
            var aimDuration = actualDurationAsTimeSpan ?? plannedDurationAsTimeSpan;
            var collectionPeriodReferenceDate = DateFromCollectionPeriod(collectionPeriod);

            return (!GetInvalidCompletionStatuses().Contains(completionStatus) ||
                   aimReference == "ZPROG001" && completionStatus == CompletionStatus.Completed) &&
                   aimStartPeriod.AcademicYear == collectionPeriod.AcademicYear && 
                   aimStartDate + aimDuration >= collectionPeriodReferenceDate;
        }

        private static DateTime DateFromCollectionPeriod(CollectionPeriod collectionPeriod)
        {
            var year = collectionPeriod.AcademicYear / 100 + 2000;

            return collectionPeriod.Period < 6 
                ? new DateTime(year, collectionPeriod.Period + 7, 1) 
                : new DateTime(year + 1, collectionPeriod.Period - 5, 1);
        }

        private static IEnumerable<CompletionStatus> GetInvalidCompletionStatuses()
        {
            yield return CompletionStatus.Completed;
            yield return CompletionStatus.Withdrawn;
            yield return CompletionStatus.BreakInLearning;
        }
    }
}