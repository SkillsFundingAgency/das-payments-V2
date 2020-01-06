using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.RequiredPayments
{
    [TestFixture]
    public class CalculatedRequiredCoInvestedAmountMappingTests : RequiredPaymentsMappingTests<CalculatedRequiredCoInvestedAmount>
    {
        protected override CalculatedRequiredCoInvestedAmount CreatePaymentEvent()
        {
            return new CalculatedRequiredCoInvestedAmount
            {
                ContractType = ContractType.Act1,
                ApprenticeshipId = 400L,
                ApprenticeshipPriceEpisodeId = 800L
            };
        }

        [TestCase(ContractType.Act1)]
        [TestCase(ContractType.Act2)]
        public void Maps_ContractType(ContractType contractType)
        {
            var payment = CreatePaymentEvent();
            payment.ContractType = contractType;

            Mapper.Map<RequiredPaymentEventModel>(payment).ContractType.Should().Be(payment.ContractType);
        }
    }
}