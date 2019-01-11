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
    public class IncentiveRequiredPaymentMappingTests: RequiredPaymentsMappingTests<IncentiveRequiredPaymentEvent>
    {
        protected override IncentiveRequiredPaymentEvent CreatePaymentEvent()
        {
            return new IncentiveRequiredPaymentEvent
            {
                ContractType = ContractType.Act2,
                
            };
        }

        [TestCaseSource(nameof(GetIncentiveTypes))]
        public void Maps_IncentiveTypes(IncentivePaymentType incentiveType)
        {
            PaymentEvent.Type = incentiveType;
            var model = Mapper.Map<RequiredPaymentEventModel>(PaymentEvent);
            model.TransactionType.Should().Be((TransactionType)PaymentEvent.Type);
        }

        public static Array GetIncentiveTypes()
        {
            return GetEnumValues<IncentivePaymentType>();
        }

        [TestCaseSource(nameof(GetContractTypes))]
        public void Maps_ContractType(ContractType contractType)
        {
            PaymentEvent.ContractType = contractType;
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).ContractType.Should().Be(PaymentEvent.ContractType);
        }

        [Test]
        public void Maps_SfaContributionPercentage()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).SfaContributionPercentage.Should().Be(1);
        }
    }
}