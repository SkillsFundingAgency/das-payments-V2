using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class RequiredPaymentMappingTests
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;

        [SetUp]
        public void Setup()
        {
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }
       
        [TestCase(typeof(CalculatedRequiredIncentiveAmount), typeof(SfaFullyFundedFundingSourcePaymentEvent))]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount), typeof(EmployerCoInvestedFundingSourcePaymentEvent))]
        [TestCase(typeof(CalculatedRequiredCoInvestedAmount), typeof(SfaCoInvestedFundingSourcePaymentEvent))]
        [TestCase(typeof(CalculatedRequiredLevyAmount), typeof(LevyFundingSourcePaymentEvent))]
        public void PriceEpisodeMapsEarningsInfo(Type requiredPaymentEventType, Type fundingSourceEventType)
        {
            var requiredPaymentEvent = Activator.CreateInstance(requiredPaymentEventType) as RequiredPaymentEvent;
            requiredPaymentEvent.StartDate = DateTime.UtcNow;
            requiredPaymentEvent.PlannedEndDate = DateTime.UtcNow;
            requiredPaymentEvent.ActualEndDate = DateTime.UtcNow;
            requiredPaymentEvent.CompletionStatus = 6;
            requiredPaymentEvent.CompletionAmount = 100M;
            requiredPaymentEvent.InstalmentAmount = 50M;
            requiredPaymentEvent.NumberOfInstalments = 8;

            var fundingSourceEvent = Activator.CreateInstance(fundingSourceEventType) as FundingSourcePaymentEvent;

            autoMapper.Map(requiredPaymentEvent, fundingSourceEvent);

            fundingSourceEvent.StartDate.Should().Be(requiredPaymentEvent.StartDate);
            fundingSourceEvent.PlannedEndDate.Should().Be(requiredPaymentEvent.PlannedEndDate);
            fundingSourceEvent.ActualEndDate.Should().Be(requiredPaymentEvent.ActualEndDate);
            fundingSourceEvent.CompletionStatus.Should().Be(requiredPaymentEvent.CompletionStatus);
            fundingSourceEvent.CompletionAmount.Should().Be(requiredPaymentEvent.CompletionAmount);
            fundingSourceEvent.InstalmentAmount.Should().Be(requiredPaymentEvent.InstalmentAmount);
            fundingSourceEvent.NumberOfInstalments.Should().Be(requiredPaymentEvent.NumberOfInstalments);
        }
    }
}