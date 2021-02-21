using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class ProviderPaymentModelToProviderPaymentEventMappingTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile<ProviderPaymentsProfile>(); });
            Mapper.AssertConfigurationIsValid();
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