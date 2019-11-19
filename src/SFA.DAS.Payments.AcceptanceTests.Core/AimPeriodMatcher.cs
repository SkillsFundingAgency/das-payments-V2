using System;
using System.Linq;
using System.Text.RegularExpressions;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Tests.Core;

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
            if (!actualDurationAsTimeSpan.HasValue)
            {
                return true;
            }

            var academicYearStart = new DateTime(collectionPeriod.AcademicYear / 100 + 2000, 8, 1);
            if (startDate.ToDate().Add(actualDurationAsTimeSpan.Value) < academicYearStart)
            {
                return false;
            }

            return true;
        }

        public static CollectionPeriod GetEndPeriodForAim(CollectionPeriod aimStartDate, string duration)
        {
            var startPeriod = aimStartDate.Period;

            var options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);
            var cleanStr = regex.Replace(duration, " ");

            var durationElements = cleanStr.Split(' ');

            if (Array.Exists(durationElements, x => x.Contains("month")))
            {
                var months = int.Parse(durationElements[0]);

                if (durationElements.Contains("-") && Array.Exists(durationElements, x => x.Contains("day")))
                {
                    // for now assume the number of days is irrelevant as it should never go back more than 1 month, so no calc required.
                    months -= 1;
                }

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