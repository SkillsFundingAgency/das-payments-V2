using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping
{
    public abstract class FundingSourceMappingTests<TSource> : PeriodisedEventMappingTests<TSource, FundingSourceEventModel>
        where TSource : FundingSourcePaymentEvent
    {

        protected override void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<FundingSourceProfile>();
        }

        [Test]
        public void Maps_SfaContributionPercentage()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).SfaContributionPercentage.Should().Be(PaymentEvent.SfaContributionPercentage);
        }

        [TestCaseSource(nameof(GetContractTypes))]
        public void Maps_ContractType(ContractType contractType)
        {
            PaymentEvent.ContractType = contractType;
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).ContractType.Should().Be(PaymentEvent.ContractType);
        }
        public static Array GetContractTypes()
        {
            return Enum.GetValues(typeof(ContractType));
        }

        [TestCaseSource(nameof(GetTransactionTypes))]
        public void Maps_TransactionType(TransactionType transactionType)
        {
            PaymentEvent.TransactionType = transactionType;
            var model = Mapper.Map<FundingSourceEventModel>(PaymentEvent);
            model.TransactionType.Should().Be(PaymentEvent.TransactionType);
        }

        public static Array GetTransactionTypes()
        {
            return Enum.GetValues(typeof(TransactionType));
        }

        [TestCaseSource(nameof(GetFundingSource))]
        public void Maps_FundingSource(FundingSourceType fundingSource)
        {
            PaymentEvent.FundingSourceType = fundingSource;
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).FundingSource.Should().Be(PaymentEvent.FundingSourceType);
        }
        public static Array GetFundingSource()
        {
            return Enum.GetValues(typeof(FundingSourceType));
        }

        [Test]
        public void Maps_RequiredPaymentEventId()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).RequiredPaymentEventId.Should().Be(PaymentEvent.RequiredPaymentEventId);
        }

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.RequiredPaymentEventId = Guid.NewGuid();
            paymentEvent.SfaContributionPercentage = .9M;
        }
    }
}