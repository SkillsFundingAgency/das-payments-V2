using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.Services
{
    [TestFixture]
    public class CoInvestmentCalculationServiceTests
    {
        private ICoInvestmentCalculationService service;
        private static DateTime recalculationStartDate;
        private PayableEarningEvent payableEvent;

        [SetUp]
        public void SetUp()
        {
            service = new CoInvestmentCalculationService();
            recalculationStartDate = new DateTime(2024, 4, 1);
            payableEvent = new PayableEarningEvent();

        }


        [Test]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        public void IsEligibleForRecalculation_ShouldNotAllowRecalcBeforeStartDate(int dateModifier, bool requiresRecalc)
        {
            payableEvent.StartDate = recalculationStartDate.AddDays(dateModifier);
            payableEvent.AgeAtStartOfLearning = 21;

            var result = service.IsEligibleForRecalculation(payableEvent);

            result.Should().Be(requiresRecalc);
        }

        [Test]
        [TestCase(21, true)]
        [TestCase(22, false)]
        [TestCase(23, false)]
        public void IsEligibleForRecalculation_ShouldNotAllowApprentice22OrOver(int apprenticeAge, bool isCorrectAge)
        {
            payableEvent.StartDate = recalculationStartDate;
            payableEvent.AgeAtStartOfLearning = apprenticeAge;

            var result = service.IsEligibleForRecalculation(payableEvent);

            result.Should().Be(isCorrectAge);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(22, true)]
        [TestCase(null, true)]
        [TestCase(0, false)]
        public void ProcessPeriodsForRecalculation_ShouldNotRecalcForNullApprenticeIdsOrDataLocks(long? apprenticeId, bool DLFailure)
        {

            var dataLockFailures = new List<DataLockFailure>{new DataLockFailure()};
            var periods = new List<(EarningPeriod period, int type)>
            {
                (new EarningPeriod { ApprenticeshipId = apprenticeId, DataLockFailures = DLFailure ? dataLockFailures : null, SfaContributionPercentage = 0} , 1)
            };

            var result = service.ProcessPeriodsForRecalculation(periods);

            result.FirstOrDefault().period.SfaContributionPercentage.Should().Be(0);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetEarningType_ReturnsLevyWhenOnProgrammeEarningType(int typeEnum)
        {
            var result = service.GetEarningType(typeEnum);

            result.Should().Be(EarningType.Levy);
        }

        [Test]
        public void GetEarningType_ReturnsLevyWhenNotOnProgrammeEarningType()
        {
            var result = service.GetEarningType(-1);

            result.Should().Be(EarningType.Incentive);
        }

    }
}
