using System;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.FundingSource
{
    [TestFixture]
    public class LevyFundedFundingSourcePaymentEventTests : FundingSourceMappingTests<LevyFundingSourcePaymentEvent>
    {
        protected override LevyFundingSourcePaymentEvent CreatePaymentEvent()
        {
            return new LevyFundingSourcePaymentEvent
            {
                RequiredPaymentEventId = Guid.NewGuid()
            };
        }
    }
}