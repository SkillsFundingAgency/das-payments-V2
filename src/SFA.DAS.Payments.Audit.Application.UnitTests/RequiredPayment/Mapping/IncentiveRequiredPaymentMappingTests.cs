﻿using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.RequiredPayment.Mapping
{
    [TestFixture]
    public class IncentiveRequiredPaymentMappingTests: RequiredPaymentsMappingTests<CalculatedRequiredIncentiveAmount>
    {
        protected override CalculatedRequiredIncentiveAmount CreatePaymentEvent()
        {
            return new CalculatedRequiredIncentiveAmount
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