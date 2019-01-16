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
            var aimStartDate = startDate.ToDate();
            var aimStartPeriod = new CollectionPeriodBuilder().WithDate(aimStartDate).Build();
            var aimDuration = actualDurationAsTimeSpan ?? plannedDurationAsTimeSpan;
            var collectionPeriodReferenceDate = DateFromCollectionPeriodName(collectionPeriod.Name);

            return aimStartPeriod.AcademicYear == collectionPeriod.AcademicYear ||
                   (aimStartDate + aimDuration >= collectionPeriodReferenceDate && completionStatus != CompletionStatus.Completed) ||
                   (aimStartDate + aimDuration >= collectionPeriodReferenceDate && aimReference == "ZPROG001" && completionStatus == CompletionStatus.Completed);
        }

        private static DateTime DateFromCollectionPeriodName(string collectionPeriodName)
        {
            var year = int.Parse(collectionPeriodName.Substring(0, 2)) + 2000;
            var period = int.Parse(collectionPeriodName.Substring(6));

            if (period < 6)
            {
                return new DateTime(year, period + 7, 1);
            }
            return new DateTime(year + 1, period - 5, 1);
        }
    }
}