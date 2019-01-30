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
    public class ApprenticeshipContractType1RequiredPaymentMappingTests : RequiredPaymentsMappingTests<ApprenticeshipContractType1RequiredPaymentEvent>
    {
        private const string ExpectedAgreementId = "OXFORD00001";

        protected override ApprenticeshipContractType1RequiredPaymentEvent CreatePaymentEvent()
        {
            return new ApprenticeshipContractType1RequiredPaymentEvent
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