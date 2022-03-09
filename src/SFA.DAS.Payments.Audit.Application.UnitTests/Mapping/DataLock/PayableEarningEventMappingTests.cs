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
    public class PayableEarningEventMappingTests : DataLockEventMappingTests<PayableEarningEvent>
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
            TransferSenderAccountId = 12345678
        };

        protected override PayableEarningEvent CreatePaymentEvent()
        {
            return new PayableEarningEvent
            {
                StartDate = DateTime.Today,
                AgreementId = "A-100",
                OnProgrammeEarnings = new List<OnProgrammeEarning> { new OnProgrammeEarning { Type = OnProgrammeEarningType.Learning, Periods = new List<EarningPeriod> { earningPeriod }.AsReadOnly() } },
                IncentiveEarnings = new List<IncentiveEarning> { new IncentiveEarning { Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod> { earningPeriod }.AsReadOnly() } }
            };
        }


        [Test]
        public void Maps_IsPayable_ToTrue()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).IsPayable.Should().BeTrue();
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
        public void Maps_OnProgrammeEarnings()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods.Count(npp => npp.TransactionType <= TransactionType.Completion).Should().Be(1);
        }

        [Test]
        public void Maps_IncentiveEarnings()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods.Count(npp => npp.TransactionType > TransactionType.Completion).Should().Be(1);
        }

        [Test]
        public void Maps_Payable_OnProgramme_Periods()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods
                .Where(npp => npp.TransactionType <= TransactionType.Completion)
                .Should().AllBeEquivalentTo(new {
                    Amount = earningPeriod.Amount,
                    DeliveryPeriod = earningPeriod.Period,
                    TransactionType = TransactionType.Learning,
                    ApprenticeshipId = earningPeriod.ApprenticeshipId,
                    SfaContributionPercentage = earningPeriod.SfaContributionPercentage,
                    ApprenticeshipPriceEpisodeId = earningPeriod.ApprenticeshipPriceEpisodeId,
                    DataLockEventId = PaymentEvent.EventId,
                    LearningStartDate = PaymentEvent.LearningAim.StartDate,
                    PriceEpisodeIdentifier = earningPeriod.PriceEpisodeIdentifier,
                    AcademicYear = PaymentEvent.CollectionPeriod.AcademicYear,
                    CollectionPeriod = PaymentEvent.CollectionPeriod.Period
                });
        }

        [Test]
        public void Maps_Payable_Incentive_Periods()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods
                .Where(npp => npp.TransactionType > TransactionType.Completion)
                .All(period =>
                    period.Amount == earningPeriod.Amount &&
                    period.DeliveryPeriod == earningPeriod.Period &&
                    period.TransactionType == TransactionType.Balancing16To18FrameworkUplift &&
                    period.ApprenticeshipId == earningPeriod.ApprenticeshipId &&
                    period.SfaContributionPercentage == earningPeriod.SfaContributionPercentage &&
                    period.ApprenticeshipPriceEpisodeId == earningPeriod.ApprenticeshipPriceEpisodeId &&
                    period.DataLockEventId == PaymentEvent.EventId &&
                    period.LearningStartDate == PaymentEvent.LearningAim.StartDate &&
                    period.PriceEpisodeIdentifier == earningPeriod.PriceEpisodeIdentifier &&
                    period.AcademicYear == PaymentEvent.CollectionPeriod.AcademicYear &&
                    period.CollectionPeriod == PaymentEvent.CollectionPeriod.Period)
                .Should()
                .BeTrue();

        }

    }
}
