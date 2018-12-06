using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.PaymentsDue
{
    public class IncentivePaymentsDueMappingTests: PaymentsDueMappingTests<IncentivePaymentDueEvent>
    {
        protected override IncentivePaymentDueEvent CreatePaymentEvent()
        {
            return new IncentivePaymentDueEvent
            {
                ContractType = ContractType.Act2,
                
            };
        }

        [TestCaseSource(nameof(GetIncentiveTypes))]
        public void Maps_IncentiveTypes(IncentiveType incentiveType)
        {
            PaymentEvent.Type = incentiveType;
            var model = Mapper.Map<PaymentsDueEventModel>(PaymentEvent);
            model.TransactionType.Should().Be((TransactionType)PaymentEvent.Type);
        }

        public static Array GetIncentiveTypes()
        {
            return GetEnumValues<IncentiveType>();
        }

        [TestCaseSource(nameof(GetContractTypes))]
        public void Maps_ContractType(ContractType contractType)
        {
            PaymentEvent.ContractType = contractType;
            Mapper.Map<PaymentsDueEventModel>(PaymentEvent).ContractType.Should().Be(PaymentEvent.ContractType);
        }

        [Test]
        public void Maps_SfaContributionPercentage()
        {
            Mapper.Map<PaymentsDueEventModel>(PaymentEvent).SfaContributionPercentage.Should().Be(1);
        }
    }
}