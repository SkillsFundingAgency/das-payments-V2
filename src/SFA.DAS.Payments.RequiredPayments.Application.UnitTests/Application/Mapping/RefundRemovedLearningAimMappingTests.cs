using System;
using Autofac.Extras.Moq;
using AutoMapper;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Mapping
{
    [TestFixture]
    public class RefundRemovedLearningAimMappingTests
    {
        private AutoMock mocker;
        private IdentifiedRemovedLearningAim identifiedLearningAim;
        private PaymentHistoryEntity historicPayment;
        private IMapper mapper;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);


            identifiedLearningAim = new IdentifiedRemovedLearningAim
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "learner-ref-123",
                    Uln = 2
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learning-ref-456",
                    StandardCode = 6
                },
                Ukprn = 7
            };

            historicPayment = new PaymentHistoryEntity
            {
                Amount = 10,
                SfaContributionPercentage = .9M,
                TransactionType = (int)OnProgrammeEarningType.Learning,
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                LearnAimReference = "aim-ref-123",
                LearnerReferenceNumber = "learning-ref-456",
                PriceEpisodeIdentifier = "pe-1",
                DeliveryPeriod = 1,
                Ukprn = 7,
                ActualEndDate = null,
                CompletionAmount = 3000,
                CompletionStatus = 1,
                ExternalId = Guid.NewGuid(),
                FundingSource = FundingSourceType.Levy,
                InstalmentAmount = 1000,
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.Today,
                StartDate = DateTime.Today.AddMonths(-12),
            };
        }

        [Test]
        public void Maps_Required_Fields_From_Identified_Learner()
        {
            var requiredPayment = new CalculatedRequiredLevyAmount();
            mapper.Map(identifiedLearningAim, requiredPayment);                                                           
            requiredPayment.ShouldBeMappedTo(identifiedLearningAim);
        }

        [Test]
        public void Maps_Required_Fields_From_Historic_Payment()
        {
            var requiredPayment = new CalculatedRequiredLevyAmount();
            mapper.Map(historicPayment, requiredPayment);
            requiredPayment.ShouldBeMappedTo(historicPayment  );
        }
    }
}