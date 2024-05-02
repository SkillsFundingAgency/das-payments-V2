using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
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
        public void IsEligibleForRecalculation_Should_Not_Allow_Recalc_Before_StartDate(int dateModifier, bool requiresRecalc)
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
        [TestCase(null, false)]
        public void IsEligibleForRecalculation_Should_Not_Allow_Apprentice_22_Or_Over(int? apprenticeAge, bool isCorrectAge)
        {
            payableEvent.StartDate = recalculationStartDate;
            payableEvent.AgeAtStartOfLearning = apprenticeAge;

            var result = service.IsEligibleForRecalculation(payableEvent);

            result.Should().Be(isCorrectAge);
        }

        [Test]
        [TestCase(null, 21)]
        [TestCase("2024/03/02", 21)]
        [TestCase("2024/04/01", 23)]
        [TestCase("2024/04/01", null)]

        public void ProcessPeriodsForRecalculation_Should_Not_Recalc_For_Invalid_Events(DateTime? eventStartDate, int? ageAtStartOfLearning)
        {

            var dataLockFailures = new List<DataLockFailure> { new DataLockFailure() };
            var periods = new List<(EarningPeriod period, int type)>
            {
                (new EarningPeriod { ApprenticeshipId = 1234, DataLockFailures = null, SfaContributionPercentage = new decimal(0.95)} , 1)
            };

            var result = service.ProcessPeriodsForRecalculation(payableEvent, periods);

            result.FirstOrDefault().period.SfaContributionPercentage.Should().Be(new decimal(0.95));
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(22, true)]
        [TestCase(null, true)]
        [TestCase(0, false)]
        public void ProcessPeriodsForRecalculation_Should_Not_Recalc_For_Null_Apprentice_Ids_Or_DataLocks(long? apprenticeId, bool DLFailure)
        {

            var dataLockFailures = new List<DataLockFailure>{new DataLockFailure()};
            var periods = new List<(EarningPeriod period, int type)>
            {
                (new EarningPeriod { ApprenticeshipId = apprenticeId, DataLockFailures = DLFailure ? dataLockFailures : null, SfaContributionPercentage = 0} , 1)
            };

            var result = service.ProcessPeriodsForRecalculation(payableEvent, periods);

            result.FirstOrDefault().period.SfaContributionPercentage.Should().Be(0);
        }

        [Test]
        [TestCase(ApprenticeshipEmployerType.Levy, 0.95)]
        public void ProcessPeriodsForRecalculation_Should_Not_Override_CoInvestmentRate_For_Levy_Employers(ApprenticeshipEmployerType apprenticeshipEmployerType, decimal? fundingPercentage)
        {
            payableEvent.StartDate = recalculationStartDate;
            payableEvent.AgeAtStartOfLearning = 21;
            var periods = new List<(EarningPeriod period, int type)>
            {
                (new EarningPeriod { ApprenticeshipId = 1234, ApprenticeshipEmployerType = apprenticeshipEmployerType, SfaContributionPercentage = fundingPercentage} , 1)
            };

            var result = service.ProcessPeriodsForRecalculation(payableEvent, periods);

            result.FirstOrDefault().period.SfaContributionPercentage.Should().Be(fundingPercentage);
        }

        [Test]
        [TestCase(ApprenticeshipEmployerType.NonLevy, 1.0)]
        public void ProcessPeriodsForRecalculation_Should_Override_CoInvestmentRate_For_NonLevy_Employers(ApprenticeshipEmployerType apprenticeshipEmployerType, decimal? fundingPercentage)
        {
            payableEvent.StartDate = recalculationStartDate;
            payableEvent.AgeAtStartOfLearning = 21;
            var periods = new List<(EarningPeriod period, int type)>
            {
                (new EarningPeriod { ApprenticeshipId = 1234, ApprenticeshipEmployerType = apprenticeshipEmployerType, SfaContributionPercentage = fundingPercentage} , 1)
            };

            var result = service.ProcessPeriodsForRecalculation(payableEvent, periods);

            result.FirstOrDefault().period.SfaContributionPercentage.Should().Be(fundingPercentage);
        }
    }
}
