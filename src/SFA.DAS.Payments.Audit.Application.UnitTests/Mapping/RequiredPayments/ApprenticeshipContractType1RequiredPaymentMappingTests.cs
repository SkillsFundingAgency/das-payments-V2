using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.RequiredPayments
{
    [TestFixture]
    public class ApprenticeshipContractType1RequiredPaymentMappingTests : RequiredPaymentsMappingTests<CalculatedRequiredLevyAmount>
    {
        private const string ExpectedAgreementId = "OXFORD00001";

        protected override CalculatedRequiredLevyAmount CreatePaymentEvent()
        {
            return new CalculatedRequiredLevyAmount
            {
                AgreementId = ExpectedAgreementId
            };
        }
        
        [Test]
        public void Maps_AgreementId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).AgreementId.Should().Be(ExpectedAgreementId);
        }
    }
}