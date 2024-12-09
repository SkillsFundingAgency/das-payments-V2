using System.Collections.Generic;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Builders;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Builders
{
    [TestFixture]
    public class FundingSourcePaymentEventBuilderMappingTests
    {
        private Mock<IPaymentProcessor> processor;
        private IMapper mapper;
        private FundingSourcePaymentEventBuilder builder;
        private long jobId;
        private long employerAccountId;
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
            jobId = fixture.Create<long>();
            employerAccountId = fixture.Create<long>();

            processor = new Mock<IPaymentProcessor>();
            var mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            mapper = mapperConfiguration.CreateMapper();

            builder = new FundingSourcePaymentEventBuilder(mapper, processor.Object);
        }

        [TestCase(FundingPlatformType.SubmitLearnerData)]
        [TestCase(FundingPlatformType.DigitalApprenticeshipService)]
        public void Builder_Maps_FundingPlatformType_Correctly(FundingPlatformType fundingPlatformType)
        {
            var calculatedLevyAmount = new CalculatedRequiredLevyAmount
            {
                FundingPlatformType = fundingPlatformType
            };

            var fundingSourcePayments = new List<FundingSourcePayment>
            {
                new SfaCoInvestedPayment(),
                new EmployerCoInvestedPayment()
            };

            processor.Setup(x => x.Process(It.IsAny<RequiredPayment>())).Returns(fundingSourcePayments.AsReadOnly());

            var fundingSourceEvents = builder.BuildFundingSourcePaymentsForRequiredPayment(calculatedLevyAmount, employerAccountId, jobId);

            foreach (var fundingSourceEvent in fundingSourceEvents)
            {
                fundingSourceEvent.FundingPlatformType.Should().Be(fundingPlatformType);
            }
        }
    }
}
