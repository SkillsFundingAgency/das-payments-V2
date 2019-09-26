using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    public abstract class FunctionalSkillDataLockEventMappingTests <T> : DataLockEventMappingTests<T> 
        where T : FunctionalSkillDataLockEvent, new()
    {
        protected override void PopulateCommonProperties(T paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.StartDate = DateTime.Today;
            paymentEvent.Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(new List<FunctionalSkillEarning>() {new FunctionalSkillEarning()});
        }

        [Test]
        public void Maps_StartDate()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).StartDate.Should().Be(PaymentEvent.StartDate);
        }

        [Test]
        public void Maps_Earnings()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).Earnings.Count.Should().Be(1);
        }
    }
}
