﻿using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core
{
    public static class AimPeriodMatcher
    {
        public static bool IsStartDateValidForCollectionPeriod(
            string startDate, 
            CalendarPeriod collectionPeriod, 
            TimeSpan? plannedDurationAsTimeSpan, 
            TimeSpan? actualDurationAsTimeSpan, 
            CompletionStatus completionStatus, 
            string aimReference)
        {
            var aimStartDate = startDate.ToDate();
            var aimStartPeriod = aimStartDate.ToCalendarPeriod();
            var aimDuration = actualDurationAsTimeSpan ?? plannedDurationAsTimeSpan;
            var collectionPeriodReferenceDate =
                new DateTime(collectionPeriod.Year, collectionPeriod.Month, aimStartDate.Day);

            return aimStartPeriod.AcademicYear == collectionPeriod.AcademicYear ||
                   (aimStartDate + aimDuration >= collectionPeriodReferenceDate && completionStatus != CompletionStatus.Completed) ||
                   (aimStartDate + aimDuration >= collectionPeriodReferenceDate && aimReference == "ZPROG001" && completionStatus == CompletionStatus.Completed);
        }
    }
}