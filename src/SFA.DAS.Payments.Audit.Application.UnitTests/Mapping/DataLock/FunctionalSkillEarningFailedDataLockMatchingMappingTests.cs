using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class FunctionalSkillEarningFailedDataLockMatchingMappingTests : FunctionalSkillDataLockEventMappingTests<FunctionalSkillEarningFailedDataLockMatching>
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
            DataLockFailures = new List<DataLockFailure>
            {
                new DataLockFailure { DataLockError = DataLockErrorCode.DLOCK_02, ApprenticeshipId = 123}
            }
        };

        protected override FunctionalSkillEarningFailedDataLockMatching CreatePaymentEvent()
        {
            return new FunctionalSkillEarningFailedDataLockMatching
            {
                Earnings = new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                        Periods = new List<EarningPeriod>{earningPeriod}.AsReadOnly()
                    }
                }.AsReadOnly()
            };
        }

        [Test]
        public void Maps_IsPayable_ToFalse()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).IsPayable.Should().BeFalse();
        }

        [Test]
        public void Maps_NonPayable_Earnings_Periods()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).NonPayablePeriods.Count(period => period.TransactionType > TransactionType.Completion).Should().Be(1);
            var periodToTest = Mapper.Map<DataLockEventModel>(PaymentEvent).NonPayablePeriods
                .FirstOrDefault(npp => npp.TransactionType > TransactionType.Completion);
            periodToTest.Should().NotBeNull();

            periodToTest.Amount.Should().Be(earningPeriod.Amount);
            periodToTest.DeliveryPeriod.Should().Be(earningPeriod.Period);
            periodToTest.TransactionType.Should().Be(TransactionType.OnProgrammeMathsAndEnglish);
            periodToTest.SfaContributionPercentage.Should().Be(earningPeriod.SfaContributionPercentage);
            periodToTest.DataLockEventId.Should().Be(PaymentEvent.EventId);
            periodToTest.LearningStartDate.Should().Be(PaymentEvent.LearningAim.StartDate);
            periodToTest.PriceEpisodeIdentifier.Should().Be(earningPeriod.PriceEpisodeIdentifier);
            periodToTest.Failures.Should().NotBeNullOrEmpty();
            periodToTest.Failures.FirstOrDefault().ApprenticeshipId.Should().Be(123);
            periodToTest.Failures.FirstOrDefault().DataLockFailure.Should().Be(DataLockErrorCode.DLOCK_02);
        }
    }
}
