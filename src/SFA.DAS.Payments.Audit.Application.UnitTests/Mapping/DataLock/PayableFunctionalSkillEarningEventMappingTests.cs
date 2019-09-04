using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    [TestFixture]
    public class PayableFunctionalSkillEarningEventMappingTests : FunctionalSkillDataLockEventMappingTests<PayableFunctionalSkillEarningEvent>
    {
        protected override PayableFunctionalSkillEarningEvent CreatePaymentEvent()
        {
            return new PayableFunctionalSkillEarningEvent
                   {
                       StartDate = DateTime.Today
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
            Mapper.Map<DataLockEventModel>(PaymentEvent).StartDate.Should().Be(DateTime.Today);
        }
    }
}
