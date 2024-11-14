using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class CoInvestmentAndFullyFundedFundingSourceMappingTests
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        [TestCase(FundingPlatformType.SubmitLearnerData)]
        [TestCase(FundingPlatformType.DigitalApprenticeshipService)]
        public void CalculateRequiredCoInvestedAmount_Maps_FundingPlatform_For_EmployerCoInvestedFundingSourcePaymentEvent(FundingPlatformType fundingPlatformType)
        {
            var calculatedRequiredCoInvestmentAmount = new CalculatedRequiredCoInvestedAmount
            {
                FundingPlatformType = fundingPlatformType
            };

            var employerCoInvestedFundingSourcePaymentEvent = autoMapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(calculatedRequiredCoInvestmentAmount);

            employerCoInvestedFundingSourcePaymentEvent.FundingPlatformType.Should().Be(calculatedRequiredCoInvestmentAmount.FundingPlatformType);
        }

        [TestCase(FundingPlatformType.SubmitLearnerData)]
        [TestCase(FundingPlatformType.DigitalApprenticeshipService)]
        public void CalculateRequiredCoInvestedAmount_Maps_FundingPlatform_For_SfaCoInvestedFundingSourcePaymentEvent(FundingPlatformType fundingPlatformType)
        {
            var calculatedRequiredCoInvestmentAmount = new CalculatedRequiredCoInvestedAmount
            {
                FundingPlatformType = fundingPlatformType
            };

            var sfaCoInvestedFundingSourcePaymentEvent = autoMapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(calculatedRequiredCoInvestmentAmount);

            sfaCoInvestedFundingSourcePaymentEvent.FundingPlatformType.Should().Be(calculatedRequiredCoInvestmentAmount.FundingPlatformType);
        }

        [Test]
        public void CalculatedRequiredIncentiveAmount_Maps_FundingPlatform_For_SfaFullyFundedFundingSourcePaymentEvent()
        {
            var calculatedRequiredIncentiveAmount = new CalculatedRequiredIncentiveAmount();

            var sfaFullyFundedFundingSourcePaymentEvent = autoMapper.Map<SfaFullyFundedFundingSourcePaymentEvent>(calculatedRequiredIncentiveAmount);

            sfaFullyFundedFundingSourcePaymentEvent.FundingPlatformType.Should().Be(FundingPlatformType.SubmitLearnerData);
        }
    }
}
