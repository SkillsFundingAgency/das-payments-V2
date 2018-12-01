using System;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping
{
    [TestFixture]
    public class SfaCoInvestedFundingSourcePaymentEventTests : FundingSourceMappingTests<SfaCoInvestedFundingSourcePaymentEvent>
    {
        protected override SfaCoInvestedFundingSourcePaymentEvent CreatePaymentEvent()
        {
            return new SfaCoInvestedFundingSourcePaymentEvent
            {
                RequiredPaymentEventId = Guid.NewGuid()
            };
        }
    }
}