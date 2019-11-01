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


        [Test]
        public void Maps_ContractType()
        {
            var payment = CreatePaymentEvent();
            Mapper.Map<RequiredPaymentEventModel>(payment).ContractType.Should().Be(payment.ContractType);
        }
    }
}