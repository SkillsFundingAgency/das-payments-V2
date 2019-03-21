﻿using System;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using FluentAssertions;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Tests
{
    [TestFixture]
    public class AimPeriodMatcherTest
    {
        private const string ZProgAim = "ZPROG001";
        const string StartOfCurrentAcademicYear = "03/Aug/Current Academic Year";
        const string StartOfPreviousAcademicYear = "01/Aug/Last Academic Year";
        private readonly CollectionPeriod firstPeriod = new CollectionPeriod { AcademicYear = 1819, Period = 1 };
        private readonly CollectionPeriod fourthPeriod = new CollectionPeriod { AcademicYear = 1819, Period = 4 };
        private readonly CollectionPeriod fifthPeriod = new CollectionPeriod { AcademicYear = 1819, Period = 5 };
        private readonly CollectionPeriod sixthPeriod = new CollectionPeriod { AcademicYear = 1819, Period = 6 };
        private readonly TimeSpan annualDuration = new TimeSpan(365, 0, 0, 0);
        private readonly TimeSpan threeMonthDuration = new TimeSpan(92, 0, 0, 0);
        private readonly string threeMonths = "3 months";
        private readonly string annualMonths = "12 months";

        [Test]
        public void ContinuingZProgAimStartingInPreviousYearReturnsTrueWhenActiveInR01InCurrentYear()
        {
            var plannedDuration = new TimeSpan(730, 0, 0, 0);
            var completionStatus = CompletionStatus.Continuing;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    StartOfPreviousAcademicYear,
                    firstPeriod,
                    plannedDuration,
                    null,
                    completionStatus,
                    ZProgAim,
                    annualMonths,
                    null);

            isStartDateValid.Should().Be(true);
        }

        [Test]
        public void ContinuingZProgAimStartingInR04ReturnsFalseWhenInR01InCurrentYear()
        {
            var plannedDuration = new TimeSpan(30*9, 0, 0, 0);
            var completionStatus = CompletionStatus.Continuing;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    "03/Nov/Current Academic Year",
                    firstPeriod,
                    plannedDuration,
                    null,
                    completionStatus,
                    ZProgAim,
                    annualMonths,
                    null);

            isStartDateValid.Should().Be(false);
        }

        [Test]
        public void ContinuingZProgAimStartingInR04ReturnsTrueWhenInR04InCurrentYear()
        {
            var completionStatus = CompletionStatus.Continuing;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    "03/Nov/Current Academic Year",
                    fourthPeriod,
                    annualDuration,
                    null,
                    completionStatus,
                    ZProgAim,
                    annualMonths,
                    null);

            isStartDateValid.Should().Be(true);
        }

        [Test]
        public void WithdrawnZProgAimReturnsTrueWhenWithdrawnButActiveInR01()
        {
            var completionStatus = CompletionStatus.Withdrawn;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    StartOfCurrentAcademicYear,
                    firstPeriod,
                    annualDuration,
                    threeMonthDuration,
                    completionStatus,
                    ZProgAim,
                    annualMonths,
                    threeMonths);

            isStartDateValid.Should().Be(true);
        }

        [Test]
        public void CompletedLastYearZProgAimReturnsTrueWhenInPeriodR01()
        {
            var completionStatus = CompletionStatus.Completed;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    StartOfPreviousAcademicYear,
                    firstPeriod,
                    annualDuration,
                    null,
                    completionStatus,
                    ZProgAim,
                    annualMonths,
                    null);

            isStartDateValid.Should().Be(true);
        }

        [Test]
        public void CompletedZProgAimFromPreviousAcademicYearReturnsTrue()
        {
            var completionStatus = CompletionStatus.Completed;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    StartOfPreviousAcademicYear,
                    firstPeriod,
                    annualDuration,
                    annualDuration,
                    completionStatus,
                    ZProgAim,
                    annualMonths,
                    annualMonths);

            isStartDateValid.Should().Be(true);
        }

        [Test]
        public void CompletedOtherAimFromPreviousAcademicYearReturnsFalse()
        {
            var completionStatus = CompletionStatus.Completed;

            var isStartDateValid =
                AimPeriodMatcher.IsStartDateValidForCollectionPeriod(
                    StartOfPreviousAcademicYear,
                    firstPeriod,
                    annualDuration,
                    annualDuration,
                    completionStatus,
                    "12345",
                    annualMonths,
                    annualMonths);

            isStartDateValid.Should().Be(false);
        }
    }
}
