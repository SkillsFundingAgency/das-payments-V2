using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.DataLock
{
    [TestFixture]
    public class PayableEarningEventMappingTests : DataLockEventMappingTests<PayableEarningEvent>
    {
        protected override PayableEarningEvent CreatePaymentEvent()
        {
            return new PayableEarningEvent
                   {
                       StartDate = DateTime.Today,
                       AgreementId = "A-100",
                       OnProgrammeEarnings = new List<OnProgrammeEarning>() {new OnProgrammeEarning()},
                       IncentiveEarnings = new List<IncentiveEarning>() {new IncentiveEarning()}
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

        [Test]
        public void Maps_AgreementId()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).AgreementId.Should().Be(PaymentEvent.AgreementId);
        }

        [Test]
        public void Maps_OnProgrammeEarnings()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).OnProgrammeEarnings.Count.Should().Be(1);
        }

        [Test]
        public void Maps_IncentiveEarnings()
        {
            Mapper.Map<DataLockEventModel>(PaymentEvent).IncentiveEarnings.Count.Should().Be(1);
        }
    }
}
