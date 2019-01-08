using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using FluentAssertions;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using System;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests
{
    [TestFixture]
    public class CoInvestedFundingSourcePaymentEventMapperTest
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;
        private CoInvestedFundingSourcePaymentEventMapper coInvestedFundingMapper;
        private ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentEvent;

        [SetUp]
        public void Setup()
        {
            requiredPaymentEvent = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                AmountDue = 1000.00m,
                CollectionPeriod = CollectionPeriod.CreateFromAcademicYearAndPeriod("1819", 1),
                DeliveryPeriod = DeliveryPeriod.CreateFromAcademicYearAndPeriod("1819", 1),
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
                Ukprn = 10000
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
                AmountDue = 900.00m,
                ContractType = ContractType.Act2,
                SfaContributionPercentage = requiredPaymentEvent.SfaContributionPercentage,
                CollectionPeriod = requiredPaymentEvent.CollectionPeriod,
                DeliveryPeriod = requiredPaymentEvent.DeliveryPeriod,
                EventTime = requiredPaymentEvent.EventTime,
                JobId = requiredPaymentEvent.JobId,
                Learner = requiredPaymentEvent.Learner,
                TransactionType = (TransactionType)requiredPaymentEvent.OnProgrammeEarningType,
                LearningAim = requiredPaymentEvent.LearningAim,
                PriceEpisodeIdentifier = requiredPaymentEvent.PriceEpisodeIdentifier,
                Ukprn = requiredPaymentEvent.Ukprn,
                FundingSourceType =FundingSourceType.CoInvestedSfa
            };

            var actualSfaCoInvestedPayment = coInvestedFundingMapper.MapToCoInvestedPaymentEvent(requiredPaymentEvent, coInvestedPayment);

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
                AmountDue = 100.00m,
                ContractType = ContractType.Act2,
                SfaContributionPercentage = requiredPaymentEvent.SfaContributionPercentage,
                CollectionPeriod = requiredPaymentEvent.CollectionPeriod,
                DeliveryPeriod = requiredPaymentEvent.DeliveryPeriod,
                EventTime = requiredPaymentEvent.EventTime,
                JobId = requiredPaymentEvent.JobId,
                Learner = requiredPaymentEvent.Learner,
                TransactionType = (TransactionType)requiredPaymentEvent.OnProgrammeEarningType,
                LearningAim = requiredPaymentEvent.LearningAim,
                PriceEpisodeIdentifier = requiredPaymentEvent.PriceEpisodeIdentifier,
                Ukprn = requiredPaymentEvent.Ukprn,
                FundingSourceType = FundingSourceType.CoInvestedEmployer
            };

          

            var actualEmployerCoInvestedPayment = coInvestedFundingMapper.MapToCoInvestedPaymentEvent(requiredPaymentEvent, coInvestedPayment);

            actualEmployerCoInvestedPayment.Should().BeEquivalentTo(expectedPayment);
        }

        [Test]
        public void ShouldMapToValidRequiredCoInvestedPayment()
        {
            var expectedRequiredCoInvestedPayment = new RequiredCoInvestedPayment
            {
                AmountDue = requiredPaymentEvent.AmountDue,
                SfaContributionPercentage = requiredPaymentEvent.SfaContributionPercentage
            };

            var actual = coInvestedFundingMapper.MapToRequiredCoInvestedPayment(requiredPaymentEvent);

            actual.Should().BeEquivalentTo(expectedRequiredCoInvestedPayment);

        }

    }
}