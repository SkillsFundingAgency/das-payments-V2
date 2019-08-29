using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class CoInvestedFundingSourcePaymentEventMapperTest
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;
        private CoInvestedFundingSourcePaymentEventMapper coInvestedFundingMapper;
        private CalculatedRequiredCoInvestedAmount requiredCoInvestedAmount;

        [SetUp]
        public void Setup()
        {
            requiredCoInvestedAmount = new CalculatedRequiredCoInvestedAmount
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
                AccountId = 1000000,
                ContractType = ContractType.Act2,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
            };
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
            coInvestedFundingMapper = new CoInvestedFundingSourcePaymentEventMapper(autoMapper);
        }

        [Test]
        public void ShouldMapToValidSfaCoInvestedFundingSourcePaymentEvent()
        {
            //Arrange 
            var coInvestedPayment = new SfaCoInvestedPayment
            {
                AmountDue = 900.00m,
                Type = FundingSourceType.CoInvestedSfa,
            };

            var expectedPayment = new SfaCoInvestedFundingSourcePaymentEvent
            {
                RequiredPaymentEventId = requiredCoInvestedAmount.EventId,
                AmountDue = 900.00m,
                ContractType = ContractType.Act2,
                SfaContributionPercentage = requiredCoInvestedAmount.SfaContributionPercentage,
                CollectionPeriod = requiredCoInvestedAmount.CollectionPeriod,
                DeliveryPeriod = requiredCoInvestedAmount.DeliveryPeriod,
                EventTime = requiredCoInvestedAmount.EventTime,
                JobId = requiredCoInvestedAmount.JobId,
                Learner = requiredCoInvestedAmount.Learner,
                TransactionType = (TransactionType)requiredCoInvestedAmount.OnProgrammeEarningType,
                LearningAim = requiredCoInvestedAmount.LearningAim,
                PriceEpisodeIdentifier = requiredCoInvestedAmount.PriceEpisodeIdentifier,
                Ukprn = requiredCoInvestedAmount.Ukprn,
                FundingSourceType = FundingSourceType.CoInvestedSfa,
                AccountId = 1000000,
                ApprenticeshipEmployerType = requiredCoInvestedAmount.ApprenticeshipEmployerType,
            };

            var actualSfaCoInvestedPayment = coInvestedFundingMapper.MapToCoInvestedPaymentEvent(requiredCoInvestedAmount, coInvestedPayment);
            expectedPayment.EventId = actualSfaCoInvestedPayment.EventId;
            expectedPayment.EventTime = actualSfaCoInvestedPayment.EventTime;

            actualSfaCoInvestedPayment.Should().BeEquivalentTo(expectedPayment);
        }


        [Test]
        public void ShouldMapToValidEmployerCoInvestedFundingSourcePaymentEvent()
        {
            //Arrange 
            var coInvestedPayment = new EmployerCoInvestedPayment
            {
                AmountDue = 100.00m,
                Type = FundingSourceType.CoInvestedEmployer
            };

            var expectedPayment = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                EventId = Guid.NewGuid(),
                RequiredPaymentEventId = requiredCoInvestedAmount.EventId,
                AmountDue = 100.00m,
                ContractType = ContractType.Act2,
                SfaContributionPercentage = requiredCoInvestedAmount.SfaContributionPercentage,
                CollectionPeriod = requiredCoInvestedAmount.CollectionPeriod,
                DeliveryPeriod = requiredCoInvestedAmount.DeliveryPeriod,
                EventTime = requiredCoInvestedAmount.EventTime,
                JobId = requiredCoInvestedAmount.JobId,
                Learner = requiredCoInvestedAmount.Learner,
                TransactionType = (TransactionType)requiredCoInvestedAmount.OnProgrammeEarningType,
                LearningAim = requiredCoInvestedAmount.LearningAim,
                PriceEpisodeIdentifier = requiredCoInvestedAmount.PriceEpisodeIdentifier,
                Ukprn = requiredCoInvestedAmount.Ukprn,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                AccountId = 1000000,
                ApprenticeshipEmployerType = requiredCoInvestedAmount.ApprenticeshipEmployerType,
            };



            var actualEmployerCoInvestedPayment = coInvestedFundingMapper.MapToCoInvestedPaymentEvent(requiredCoInvestedAmount, coInvestedPayment);
            expectedPayment.EventId = actualEmployerCoInvestedPayment.EventId;
            expectedPayment.EventTime = actualEmployerCoInvestedPayment.EventTime;
            actualEmployerCoInvestedPayment.Should().BeEquivalentTo(expectedPayment);
        }

        [Test]
        public void ShouldMapToValidRequiredCoInvestedPayment()
        {
            var expectedRequiredCoInvestedPayment = new RequiredCoInvestedPayment
            {
                AmountDue = requiredCoInvestedAmount.AmountDue,
                SfaContributionPercentage = requiredCoInvestedAmount.SfaContributionPercentage
            };

            var actual = coInvestedFundingMapper.MapToRequiredCoInvestedPayment(requiredCoInvestedAmount);

            actual.Should().BeEquivalentTo(expectedRequiredCoInvestedPayment);

        }

    }
}