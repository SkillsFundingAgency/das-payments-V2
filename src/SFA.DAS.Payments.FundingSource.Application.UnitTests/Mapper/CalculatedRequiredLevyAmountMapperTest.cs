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

        protected void CompareCommonProperties(CalculatedRequiredLevyAmount source, FundingSourcePaymentEvent destination)
        {
            destination.SfaContributionPercentage.Should().Be(source.SfaContributionPercentage);
            destination.RequiredPaymentEventId.Should().Be(source.EventId);
            destination.TransactionType.Should().Be((TransactionType) source.OnProgrammeEarningType);
            destination.TransferSenderAccountId.Should().Be(source.TransferSenderAccountId);
            destination.AccountId.Should().Be(source.AccountId);
            destination.ApprenticeshipId.Should().Be(source.ApprenticeshipId);
            destination.ApprenticeshipPriceEpisodeId.Should().Be(source.ApprenticeshipPriceEpisodeId);
            destination.CollectionPeriod.AcademicYear.Should().Be(source.CollectionPeriod.AcademicYear);
            destination.CollectionPeriod.Period.Should().Be(source.CollectionPeriod.Period);
            destination.ContractType.Should().Be(source.ContractType);
            destination.DeliveryPeriod.Should().Be(source.DeliveryPeriod);
            destination.EarningEventId.Should().Be(source.EarningEventId);
            destination.EventId.Should().NotBe(source.EventId);
            destination.IlrSubmissionDateTime.Should().Be(source.IlrSubmissionDateTime);
            destination.InstalmentAmount.Should().Be(source.InstalmentAmount);
            destination.NumberOfInstalments.Should().Be(source.NumberOfInstalments);
            destination.CompletionAmount.Should().Be(source.CompletionAmount);
            destination.CompletionStatus.Should().Be(source.CompletionStatus);
            destination.ActualEndDate.Should().Be(source.ActualEndDate);
            destination.PlannedEndDate.Should().Be(source.PlannedEndDate);
            destination.Learner.Should().BeEquivalentTo(source.Learner);
            destination.LearningAim.Should().BeEquivalentTo(source.LearningAim);
            destination.EventTime.Should().NotBe(source.EventTime);
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
            CompareCommonProperties(requiredPaymentEvent, actualEvent);
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
            CompareCommonProperties(requiredPaymentEvent, actualEvent);
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
            CompareCommonProperties(requiredPaymentEvent, actualEvent);
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
            CompareCommonProperties(requiredPaymentEvent, actualEvent);
        }
    }
}