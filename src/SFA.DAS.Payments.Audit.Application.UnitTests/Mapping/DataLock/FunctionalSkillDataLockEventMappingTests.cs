using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    public abstract class FunctionalSkillDataLockEventMappingTests<T> : DataLockEventMappingTests<T>
        where T : FunctionalSkillDataLockEvent, new()
    {
        //private readonly EarningPeriod earningPeriod = new EarningPeriod
        //{
        //    Period = 1,
        //    Amount = 100,
        //    AccountId = 123,
        //    ApprenticeshipId = 123456,
        //    SfaContributionPercentage = 1,
        //    ApprenticeshipPriceEpisodeId = 123,
        //    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
        //    AgreedOnDate = DateTime.Now,
        //    PriceEpisodeIdentifier = "1/1/2/2020",
        //    TransferSenderAccountId = 12345678,
        //    DataLockFailures = new List<DataLockFailure>
        //    {
        //        new DataLockFailure { DataLockError = DataLockErrorCode.DLOCK_02, ApprenticeshipId = 123}
        //    }
        //};

        protected override void PopulateCommonProperties(T paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.StartDate = DateTime.Today;
            //paymentEvent.Earnings = new List<FunctionalSkillEarning>
            //{
            //    new FunctionalSkillEarning
            //    {
            //        Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
            //        Periods = new List<EarningPeriod> {earningPeriod}.AsReadOnly()
            //    }
            //}.AsReadOnly();
        }


        [Test]
        public void Maps_StartDate()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).StartDate.Should().Be(PaymentEvent.StartDate);
        }
    }
}
