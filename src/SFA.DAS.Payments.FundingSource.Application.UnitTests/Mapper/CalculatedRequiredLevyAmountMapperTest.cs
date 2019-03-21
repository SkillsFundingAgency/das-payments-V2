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
                CommitmentId = 12,
                Priority = 13,
                EventId = Guid.NewGuid(),
                EmployerAccountId = 1000000,
                IlrSubmissionDateTime = DateTime.Today,
                EarningEventId = Guid.NewGuid(),
                ContractType = ContractType.Act1,
            };
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        [Test]
        public void TestMapToLevyFundingSourcePaymentEvent()
        {
            var expectedEvent = new LevyFundingSourcePaymentEvent
            {
                AgreementId = "11"
            };

            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
        }

        [Test]
        public void TestMapToValidSfaCoInvestedFundingSourcePaymentEvent()
        {
            var expectedEvent = new SfaCoInvestedFundingSourcePaymentEvent();
            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
        }

        [Test]
        public void TestMapToValidEmployerCoInvestedFundingSourcePaymentEvent()
        {
            var expectedEvent = new EmployerCoInvestedFundingSourcePaymentEvent();
            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
        }

        [Test]
        public void TestMapToValidSfaFullyFundedFundingSourcePaymentEvent()
        {
            var expectedEvent = new SfaFullyFundedFundingSourcePaymentEvent();
            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
        }

        private void PopulateCommonProperties(FundingSourcePaymentEvent expectedEvent)
        {
            expectedEvent.FundingSourceType = FundingSourceType.Levy;
            expectedEvent.Learner = new Learner
            {
                ReferenceNumber = "001",
                Uln = 1234567890
            };
            expectedEvent.JobId = 1;
            expectedEvent.ContractType = ContractType.Act1;
            expectedEvent.Ukprn = 10000;
            expectedEvent.AmountDue = 55;
            expectedEvent.TransactionType = TransactionType.Learning;
            expectedEvent.LearningAim = new LearningAim
            {
                FrameworkCode = 403
            };
            expectedEvent.DeliveryPeriod = 1;
            expectedEvent.IlrSubmissionDateTime = DateTime.Today;
            expectedEvent.RequiredPaymentEventId = requiredPaymentEvent.EventId;
            expectedEvent.PriceEpisodeIdentifier = "1819-P01";
            expectedEvent.SfaContributionPercentage = .9m;
            expectedEvent.CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1);
            expectedEvent.EmployerAccountId = 1000000;
        }
    }
}