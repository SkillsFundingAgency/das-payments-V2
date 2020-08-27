using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    public abstract class FunctionalSkillDataLockEventMappingTests<T> : DataLockEventMappingTests<T>
        where T : FunctionalSkillDataLockEvent, new()
    {
        protected override void PopulateCommonProperties(T paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.StartDate = DateTime.Today;
        }


        [Test]
        public void Maps_StartDate()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).StartDate.Should().Be(PaymentEvent.StartDate);
        }
    }
}
