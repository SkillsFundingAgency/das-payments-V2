using System;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.FundingSource
{
    [TestFixture]
    public class SfaFullyFundedFundingSourcePaymentEventTests : FundingSourceMappingTests<SfaFullyFundedFundingSourcePaymentEvent>
    {
        protected override SfaFullyFundedFundingSourcePaymentEvent CreatePaymentEvent()
        {
            return new SfaFullyFundedFundingSourcePaymentEvent
            {
                RequiredPaymentEventId = Guid.NewGuid()
            };
        }
    }
}