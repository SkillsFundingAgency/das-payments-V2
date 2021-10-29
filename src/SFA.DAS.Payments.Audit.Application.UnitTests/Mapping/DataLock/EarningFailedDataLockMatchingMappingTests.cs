using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    [TestFixture]
    public class EarningFailedDataLockMatchingMappingTests : DataLockEventMappingTests<EarningFailedDataLockMatching>
    {
        private readonly EarningPeriod earningPeriod = new EarningPeriod
        {
            Period = 1,
            Amount = 100,
            AccountId = 123,
            ApprenticeshipId = 123456,
            SfaContributionPercentage = 1,
            ApprenticeshipPriceEpisodeId = 123,
            ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
            AgreedOnDate = DateTime.Now,
            PriceEpisodeIdentifier = "1/1/2/2020",
            TransferSenderAccountId = 12345678, 
            DataLockFailures = new List<DataLockFailure>{new DataLockFailure{DataLockError = DataLockErrorCode.DLOCK_07, ApprenticeshipId = 123456}}
        };

        protected override EarningFailedDataLockMatching CreatePaymentEvent()
        {
            return new EarningFailedDataLockMatching
            {
                StartDate = DateTime.Today,
                AgreementId = "A-100",
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                       {
                           new OnProgrammeEarning{Type = OnProgrammeEarningType.Learning, Periods = new List<EarningPeriod> {earningPeriod}.ToList().AsReadOnly()}
                       },
                IncentiveEarnings = new List<IncentiveEarning>
                {
                    new IncentiveEarning { Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod> { earningPeriod }.ToList().AsReadOnly() }
                }
            };
        }

        [Test]
        public void Maps_IsPayable_ToFalse()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).IsPayable.Should().BeFalse();
        }

        [Test]
        public void Maps_StartDate()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).StartDate.Should().Be(PaymentEvent.StartDate);
        }

        [Test]
        public void Maps_AgreementId()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).AgreementId.Should().Be(PaymentEvent.AgreementId);
        }

        [Test]
        public void Maps_NonPayable_OnProgramme_Periods()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).NonPayablePeriods.Count(period => period.TransactionType <= TransactionType.Completion).Should().Be(1);
            var periodToTest = Mapper.Map<DataLockEventModel>(PaymentEvent).NonPayablePeriods
                .FirstOrDefault(npp => npp.TransactionType > TransactionType.Completion);
            periodToTest.Should().NotBeNull();

            periodToTest.Amount.Should().Be(earningPeriod.Amount);
            periodToTest.DeliveryPeriod.Should().Be(earningPeriod.Period);
            periodToTest.TransactionType.Should().Be(TransactionType.Balancing16To18FrameworkUplift);
            periodToTest.SfaContributionPercentage.Should().Be(earningPeriod.SfaContributionPercentage);
            periodToTest.DataLockEventId.Should().Be(PaymentEvent.EventId);
            periodToTest.LearningStartDate.Should().Be(PaymentEvent.LearningAim.StartDate);
            periodToTest.PriceEpisodeIdentifier.Should().Be(earningPeriod.PriceEpisodeIdentifier);

            periodToTest.Failures.Should().NotBeNullOrEmpty();
            periodToTest.Failures.FirstOrDefault().ApprenticeshipId.Should().Be(123456);
            periodToTest.Failures.FirstOrDefault().DataLockFailure.Should().Be(DataLockErrorCode.DLOCK_07);
        }

        [Test]
        public void Maps_NonPayable_Incentive_Periods()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).NonPayablePeriods.Count(period => period.TransactionType > TransactionType.Completion).Should().Be(1);
            var periodToTest = Mapper.Map<DataLockEventModel>(PaymentEvent).NonPayablePeriods
                .FirstOrDefault(npp => npp.TransactionType > TransactionType.Completion);
            periodToTest.Should().NotBeNull();

            periodToTest.Amount.Should().Be(earningPeriod.Amount);
            periodToTest.DeliveryPeriod.Should().Be(earningPeriod.Period);
            periodToTest.TransactionType.Should().Be(TransactionType.Balancing16To18FrameworkUplift);
            periodToTest.SfaContributionPercentage.Should().Be(earningPeriod.SfaContributionPercentage);
            periodToTest.DataLockEventId.Should().Be(PaymentEvent.EventId);
            periodToTest.LearningStartDate.Should().Be(PaymentEvent.LearningAim.StartDate);
            periodToTest.PriceEpisodeIdentifier.Should().Be(earningPeriod.PriceEpisodeIdentifier);
            periodToTest.Failures.Should().NotBeNullOrEmpty();
            periodToTest.Failures.FirstOrDefault().ApprenticeshipId.Should().Be(123456);
            periodToTest.Failures.FirstOrDefault().DataLockFailure.Should().Be(DataLockErrorCode.DLOCK_07);
        }
    }
}
