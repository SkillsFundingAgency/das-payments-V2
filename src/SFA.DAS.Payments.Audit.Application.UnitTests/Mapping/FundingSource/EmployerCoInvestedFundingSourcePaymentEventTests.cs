using System;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.FundingSource
{
    [TestFixture]
    public class EmployerCoInvestedFundingSourcePaymentEventTests : FundingSourceMappingTests<EmployerCoInvestedFundingSourcePaymentEvent>
    {
        protected override EmployerCoInvestedFundingSourcePaymentEvent CreatePaymentEvent()
        {
            return new EmployerCoInvestedFundingSourcePaymentEvent
            {
                RequiredPaymentEventId = Guid.NewGuid()
            };
        }
    }
}