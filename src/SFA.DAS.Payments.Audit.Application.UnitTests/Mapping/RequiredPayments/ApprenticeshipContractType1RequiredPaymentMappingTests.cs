using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.RequiredPayments
{
    [TestFixture]
    public class
        ApprenticeshipContractType1RequiredPaymentMappingTests : RequiredPaymentsMappingTests<
            CalculatedRequiredLevyAmount>
    {
        private const string ExpectedAgreementId = "OXFORD00001";
        private long ExpectedEmployerAccountId = 5000000;


        protected override CalculatedRequiredLevyAmount CreatePaymentEvent()
        {
            return new CalculatedRequiredLevyAmount
            {
                AgreementId = ExpectedAgreementId,
                EmployerAccountId = ExpectedEmployerAccountId
            };
        }

        [Test]
        public void Maps_AgreementId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).AgreementId.Should().Be(ExpectedAgreementId);
        }

        [Test]
        public void Maps_EmployerAccountId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).EmployerAccountId.Should().Be(ExpectedEmployerAccountId);
        }
    }
}
