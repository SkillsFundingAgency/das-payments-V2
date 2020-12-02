using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class ProcessUnableToFundTransferFundingSourcePaymentMapperTests
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;
        private ProcessUnableToFundTransferFundingSourcePayment unableToFundTransferFundingSourcePayment;
        private CalculatedRequiredLevyAmount expectedEvent;

        [SetUp]
        public void Setup()
        {
            var requiredPaymentEventEventId = Guid.NewGuid();
            var earningEventId = Guid.NewGuid();
            var utcNow = DateTime.UtcNow;
            
            unableToFundTransferFundingSourcePayment = new ProcessUnableToFundTransferFundingSourcePayment
            {
                EventId = requiredPaymentEventEventId,
                EarningEventId = earningEventId,
                EventTime = utcNow,
                RequiredPaymentEventId = requiredPaymentEventEventId,

                AmountDue = 1000.00m,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1),
                DeliveryPeriod = 1,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "001",
                    Uln = 1234567890
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 403
                },
                PriceEpisodeIdentifier = "1819-P01",
                SfaContributionPercentage = 0.9m,
                Ukprn = 10000,

                AccountId = 1000000,
                IlrSubmissionDateTime = DateTime.Today,

                ContractType = ContractType.Act1,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ApprenticeshipId = 12,
                CompletionAmount = 10,
                CompletionStatus = 1,
                InstalmentAmount = 10,
                StartDate = utcNow,
                TransactionType = TransactionType.Balancing,
                ActualEndDate = utcNow,
                FundingSourceType = FundingSourceType.Levy,
                IlrFileName = "Test",
                LearningStartDate = utcNow,
                NumberOfInstalments = 1,
                PlannedEndDate = utcNow,
                ApprenticeshipPriceEpisodeId = 1,
                TransferSenderAccountId = 10,
                ReportingAimFundingLineType = "Test"
            };
            
            expectedEvent = new CalculatedRequiredLevyAmount
            {
                EventId = requiredPaymentEventEventId,
                EarningEventId = earningEventId,
                EventTime = utcNow,
                
                AmountDue = 1000.00m,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1),
                DeliveryPeriod = 1,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "001",
                    Uln = 1234567890
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 403
                },
                PriceEpisodeIdentifier = "1819-P01",
                SfaContributionPercentage = 0.9m,
                Ukprn = 10000,

                AccountId = 1000000,
                IlrSubmissionDateTime = DateTime.Today,

                ContractType = ContractType.Act1,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ApprenticeshipId = 12,
                CompletionAmount = 10,
                CompletionStatus = 1,
                InstalmentAmount = 10,
                StartDate = utcNow,
                ActualEndDate = utcNow,
                IlrFileName = "Test",
                LearningStartDate = utcNow,
                NumberOfInstalments = 1,
                PlannedEndDate = utcNow,
                ApprenticeshipPriceEpisodeId = 1,
                OnProgrammeEarningType = OnProgrammeEarningType.Balancing,
                TransferSenderAccountId = 10,
                ReportingAimFundingLineType = "Test"
            };
            
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        [Test]
        public void TestMapToCalculatedRequiredLevyAmount()
        {
            // act
            var actualEvent = autoMapper.Map<CalculatedRequiredLevyAmount>(unableToFundTransferFundingSourcePayment);

            // assert
            actualEvent.Should().BeEquivalentTo(expectedEvent);

            //Null Apprenticeship Fix
            actualEvent.ApprenticeshipId.Should().Be(expectedEvent.ApprenticeshipId);
            actualEvent.ApprenticeshipPriceEpisodeId.Should().Be(expectedEvent.ApprenticeshipPriceEpisodeId);
            
            //EarningEventId
            actualEvent.EarningEventId.Should().Be(expectedEvent.EarningEventId);
            
            //RequiredPaymentEventId
            actualEvent.EventId.Should().Be(expectedEvent.EventId);
        }
    }
}
