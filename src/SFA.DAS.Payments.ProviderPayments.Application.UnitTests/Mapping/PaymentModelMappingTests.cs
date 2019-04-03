using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class PaymentModelMappingTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<ProviderPaymentsProfile>(); });
            Mapper.AssertConfigurationIsValid();
        }

        [TestCase(typeof(EmployerCoInvestedFundingSourcePaymentEvent))]
        [TestCase(typeof(SfaCoInvestedFundingSourcePaymentEvent))]
        [TestCase(typeof(SfaFullyFundedFundingSourcePaymentEvent))]
        [TestCase(typeof(LevyFundingSourcePaymentEvent))]
        public void EventModelEarningsInfoShouldBeCorrect(Type fundingSourceEventType)
        {
            var fundingSourceEvent = Activator.CreateInstance(fundingSourceEventType) as FundingSourcePaymentEvent;

            fundingSourceEvent.StartDate = DateTime.UtcNow;
            fundingSourceEvent.PlannedEndDate = DateTime.UtcNow;
            fundingSourceEvent.ActualEndDate = DateTime.UtcNow;
            fundingSourceEvent.CompletionStatus = 3;
            fundingSourceEvent.CompletionAmount = 100M;
            fundingSourceEvent.InstalmentAmount = 200M;
            fundingSourceEvent.NumberOfInstalments = 5;

            var providerPayment = Mapper.Map<ProviderPaymentEventModel>(fundingSourceEvent);

            providerPayment.StartDate.Should().Be(fundingSourceEvent.StartDate);
            providerPayment.PlannedEndDate.Should().Be(fundingSourceEvent.PlannedEndDate);
            providerPayment.ActualEndDate.Should().Be(fundingSourceEvent.ActualEndDate);
            providerPayment.CompletionStatus.Should().Be(fundingSourceEvent.CompletionStatus);
            providerPayment.CompletionAmount.Should().Be(fundingSourceEvent.CompletionAmount);
            providerPayment.InstalmentAmount.Should().Be(fundingSourceEvent.InstalmentAmount);
            providerPayment.NumberOfInstalments.Should().Be(fundingSourceEvent.NumberOfInstalments);
        }

        [TestCase(typeof(EmployerCoInvestedProviderPaymentEvent))]
        [TestCase(typeof(SfaCoInvestedProviderPaymentEvent))]
        [TestCase(typeof(SfaFullyFundedProviderPaymentEvent))]
        [TestCase(typeof(LevyProviderPaymentEvent))]
        public void ProviderModelEarningsInfoShouldBeCorrect(Type providerPaymentEventType)
        {
            var providerPaymentEvent = new PaymentModel
            {
                StartDate = DateTime.UtcNow,
                PlannedEndDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                CompletionStatus = 3,
                CompletionAmount = 100M,
                InstalmentAmount = 200M,
                NumberOfInstalments = 5
            };

            var providerPayment = Activator.CreateInstance(providerPaymentEventType) as ProviderPaymentEvent;

            Mapper.Map(providerPaymentEvent, providerPayment);

            providerPayment.StartDate.Should().Be(providerPaymentEvent.StartDate);
            providerPayment.PlannedEndDate.Should().Be(providerPaymentEvent.PlannedEndDate.Value);
            providerPayment.ActualEndDate.Should().Be(providerPaymentEvent.ActualEndDate);
            providerPayment.CompletionStatus.Should().Be(providerPaymentEvent.CompletionStatus);
            providerPayment.CompletionAmount.Should().Be(providerPaymentEvent.CompletionAmount);
            providerPayment.InstalmentAmount.Should().Be(providerPaymentEvent.InstalmentAmount);
            providerPayment.NumberOfInstalments.Should().Be(providerPaymentEvent.NumberOfInstalments);
        }
    }
}