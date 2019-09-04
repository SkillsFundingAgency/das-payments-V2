using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    [TestFixture]
    public class FunctionalSkillEarningFailedDataLockMatchingMappingTests : FunctionalSkillDataLockEventMappingTests<FunctionalSkillEarningFailedDataLockMatching>
    {
        protected override FunctionalSkillEarningFailedDataLockMatching CreatePaymentEvent()
        {
            return new FunctionalSkillEarningFailedDataLockMatching();
        }

        [Test]
        public void Maps_IsPayable_ToTrue()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).IsPayable.Should().BeFalse();
        }
    }
}
