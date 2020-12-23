using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    [TestFixture]
    public class PayableFunctionalSkillEarningEventMappingTests : FunctionalSkillDataLockEventMappingTests<PayableFunctionalSkillEarningEvent>
    {
        private readonly EarningPeriod earningPeriod = new EarningPeriod
        {
            Period = 1,
            Amount = 10,
            AccountId = 123,
            ApprenticeshipId = 123456,
            SfaContributionPercentage = 1,
            ApprenticeshipPriceEpisodeId = 123,
            ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
            AgreedOnDate = DateTime.Now,
            PriceEpisodeIdentifier = "1/1/2/2020",
            TransferSenderAccountId = 12345678
        };

        protected override PayableFunctionalSkillEarningEvent CreatePaymentEvent()
        {
            return new PayableFunctionalSkillEarningEvent
            {
                StartDate = DateTime.Today,
                Earnings = new List<FunctionalSkillEarning> {
                           new FunctionalSkillEarning {
                               Type = FunctionalSkillType.OnProgrammeMathsAndEnglish, Periods = new List<EarningPeriod>
                               {
                                   earningPeriod
                               }.AsReadOnly()
                           }
                       }.AsReadOnly()
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
        public void Maps_IncentiveEarnings()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods.Count(npp => npp.TransactionType > TransactionType.Completion).Should().Be(1);
        }

        [Test]
        public void Maps_Payable_Periods()
        {
            var mappedPeriod = Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods.FirstOrDefault();
            mappedPeriod.Should().NotBeNull();
            mappedPeriod.Amount.Should().Be(earningPeriod.Amount);
            mappedPeriod.DeliveryPeriod.Should().Be(earningPeriod.Period);
            mappedPeriod.TransactionType.Should().Be(TransactionType.OnProgrammeMathsAndEnglish);
            mappedPeriod.ApprenticeshipId.Should().Be(earningPeriod.ApprenticeshipId);
            mappedPeriod.SfaContributionPercentage.Should().Be(earningPeriod.SfaContributionPercentage);
            mappedPeriod.ApprenticeshipPriceEpisodeId.Should().Be(earningPeriod.ApprenticeshipPriceEpisodeId);
            mappedPeriod.DataLockEventId.Should().Be(PaymentEvent.EventId);
            mappedPeriod.LearningStartDate.Should().Be(PaymentEvent.LearningAim.StartDate);
            mappedPeriod.PriceEpisodeIdentifier.Should().Be(earningPeriod.PriceEpisodeIdentifier);
        }
    }
}
