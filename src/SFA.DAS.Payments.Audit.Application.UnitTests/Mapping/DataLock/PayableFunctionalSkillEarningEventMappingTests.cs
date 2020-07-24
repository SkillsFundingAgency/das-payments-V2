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
                       IncentiveEarnings = new List<IncentiveEarning> { new IncentiveEarning { Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod>
                       {
                           earningPeriod
                       }.AsReadOnly() } }
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
            Mapper.Map<DataLockEventModel>(PaymentEvent).PayablePeriods.All(period =>
                    period.Amount == earningPeriod.Amount && 
                    period.DeliveryPeriod == earningPeriod.Period &&
                    period.TransactionType == TransactionType.Balancing16To18FrameworkUplift && 
                    period.ApprenticeshipId == earningPeriod.ApprenticeshipId &&  
                    period.SfaContributionPercentage == earningPeriod.SfaContributionPercentage && 
                    period.ApprenticeshipPriceEpisodeId == earningPeriod.ApprenticeshipPriceEpisodeId && 
                    period.DataLockEventId == PaymentEvent.EventId && 
                    period.LearningStartDate == PaymentEvent.LearningAim.StartDate && 
                    period.PriceEpisodeIdentifier == earningPeriod.PriceEpisodeIdentifier)
                .Should()
                .BeTrue();
        }
    }
}
