using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class CalculatedRequiredLevyAmountMapperTest
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;
        private CalculatedRequiredLevyAmount requiredPaymentEvent;

        [SetUp]
        public void Setup()
        {
            requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                AmountDue = 1000.00m,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1),
                DeliveryPeriod = 1,
                EventTime = DateTime.UtcNow,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "001",
                    Uln = 1234567890
                },
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                LearningAim = new LearningAim
                {
                    FrameworkCode = 403
                },
                PriceEpisodeIdentifier = "1819-P01",
                SfaContributionPercentage = 0.9m,
                Ukprn = 10000,

                AgreementId = "11",
                ApprenticeshipId = 12,
                ApprenticeshipPriceEpisodeId = 123,
                Priority = 13,
                EventId = Guid.NewGuid(),
                AccountId = 1000000,
                IlrSubmissionDateTime = DateTime.Today,
                EarningEventId = Guid.NewGuid(),
                ContractType = ContractType.Act1,
            };
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        protected void CompareCommonProperties(CalculatedRequiredLevyAmount source, FundingSourcePaymentEvent destination, List<string> propertiesToIgnore)
        {
            var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            destination.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propInfo =>
                    !propertiesToIgnore.Contains(propInfo.Name)
                    && sourceProperties.Any(sourceProp => sourceProp.Name.Equals(propInfo.Name)))
                .ToList()
                .ForEach(propInfo => propInfo.GetValue(destination).Should().Be(source.GetType().GetProperty(propInfo.Name).GetValue(source),$"{propInfo.Name} didn't match."));
        }

        [Test]
        public void TestMapToLevyFundingSourcePaymentEvent()
        {
            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(requiredPaymentEvent.EventTime);
            actualEvent.EventId.Should().NotBe(requiredPaymentEvent.EventId);
            CompareCommonProperties(requiredPaymentEvent, actualEvent, new List<string> { "EventTime", "EventId", "AmountDue" });
        }

        [Test]
        public void TestMapToValidSfaCoInvestedFundingSourcePaymentEvent()
        {
            var actualEvent = new SfaCoInvestedFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.CoInvestedSfa
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(requiredPaymentEvent.EventTime);
            actualEvent.EventId.Should().NotBe(requiredPaymentEvent.EventId);
            CompareCommonProperties(requiredPaymentEvent, actualEvent, new List<string> { "EventTime", "EventId", "AmountDue" });
        }

        [Test]
        public void TestMapToValidEmployerCoInvestedFundingSourcePaymentEvent()
        {
            var actualEvent = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.CoInvestedEmployer
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(requiredPaymentEvent.EventTime);
            actualEvent.EventId.Should().NotBe(requiredPaymentEvent.EventId);
            CompareCommonProperties(requiredPaymentEvent, actualEvent, new List<string> { "EventTime", "EventId", "AmountDue" });
        }

        [Test]
        public void TestMapToValidSfaFullyFundedFundingSourcePaymentEvent()
        {
            var actualEvent = new SfaFullyFundedFundingSourcePaymentEvent()
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.FullyFundedSfa
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(requiredPaymentEvent.EventTime);
            actualEvent.EventId.Should().NotBe(requiredPaymentEvent.EventId);
            CompareCommonProperties(requiredPaymentEvent, actualEvent, new List<string> { "EventTime", "EventId", "AmountDue" });
        }
    }
}