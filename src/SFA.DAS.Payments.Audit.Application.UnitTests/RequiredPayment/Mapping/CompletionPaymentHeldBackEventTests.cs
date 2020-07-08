using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.RequiredPayment.Mapping
{
    [TestFixture]
    public class CompletionPaymentHeldBackEventTests : RequiredPaymentsMappingTests<CompletionPaymentHeldBackEvent>
    {
        protected override CompletionPaymentHeldBackEvent CreatePaymentEvent() 
            => new CompletionPaymentHeldBackEvent();

        [Test]
        public void Maps_CompletionWithheld_As_CompletionWithheldReason()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).NonPaymentReason.Should().Be(NonPaymentReason.InsufficientEmployerContribution);
        }
    }
}
