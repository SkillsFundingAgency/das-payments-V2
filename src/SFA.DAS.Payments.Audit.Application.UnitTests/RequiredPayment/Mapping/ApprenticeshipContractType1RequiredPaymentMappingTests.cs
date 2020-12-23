using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.RequiredPayment.Mapping
{
    [TestFixture]
    public class
        ApprenticeshipContractType1RequiredPaymentMappingTests : RequiredPaymentsMappingTests<
            CalculatedRequiredLevyAmount>
    {
        private const string ExpectedAgreementId = "OXFORD00001";
        private long ExpectedAccountId = 5000000;

        protected override CalculatedRequiredLevyAmount CreatePaymentEvent()
        {
            return new CalculatedRequiredLevyAmount
            {
                AgreementId = ExpectedAgreementId,
                AccountId = ExpectedAccountId,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
            };
        }

        [Test]
        public void Maps_AgreementId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).AgreementId.Should().Be(ExpectedAgreementId);
        }

        [Test]
        public void Maps_AccountId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).AccountId.Should().Be(ExpectedAccountId);
        }

        [Test]
        public void Maps_EmployerApprenticeshipType()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).ApprenticeshipEmployerType.Should().Be(ApprenticeshipEmployerType.Levy);
        }

        [Test]
        public void Maps_CompletionPaid_As_NotUnpaid()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).NonPaymentReason.Should().Be(null);
        }
    }
}