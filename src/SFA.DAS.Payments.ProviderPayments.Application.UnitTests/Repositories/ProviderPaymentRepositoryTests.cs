using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Repositories
{
    [TestFixture]
    public class ProviderPaymentRepositoryTests
    {
        private IProviderPaymentsDataContext context;
        private ProviderPaymentsRepository sut;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();

            var contextBuilder = new DbContextOptionsBuilder<ProviderPaymentsDataContext>()
                                 .UseInMemoryDatabase(databaseName: dbName)
                                 .Options;
            context = new ProviderPaymentsDataContext(contextBuilder);
            sut = new ProviderPaymentsRepository(context);
        }

        [Test]
        public async Task GetMonthEndAct1CompletionPayments_Returns_all_Act1_Completion_Payments()
        {
            var payments = CreateTestPayment();
            payments.First().Amount = 100.99m;

            context.Payment.AddRange(payments);
            await context.SaveChanges();

            var collectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 1920 };
            var actual = await sut.GetMonthEndAct1CompletionPaymentsForProvider(payments.First().Ukprn, collectionPeriod);

            actual.Should().NotBeNullOrEmpty();
            actual.Single().Amount.Should().Be(100.99m);
        }

        [Test]
        public async Task WhenGettingProvidersWithAct1CompletionPayments_ThenReturnsProviders()
        {
            var payments = CreateTestPayment();

            context.Payment.AddRange(payments);
            await context.SaveChanges();

            var collectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 1920 };
            var result = await sut.GetProvidersWithAct1CompletionPayments(collectionPeriod);

            result.Capacity.Should().Be(payments.Count);
            result.ForEach(x => x.Should().Be(12345));
        }

        private IList<PaymentModel> CreateTestPayment()
        {
            return new List<PaymentModel>
            {
                new PaymentModel
                {
                    JobId = 123,
                    EventId = Guid.NewGuid(),
                    EventTime = DateTimeOffset.UtcNow,
                    Ukprn = 12345,
                    DeliveryPeriod = 12,
                    CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 1920 },
                    LearnerUln = 123456,
                    LearnerReferenceNumber = "1234-ref",
                    PriceEpisodeIdentifier = "pe-1",
                    LearningAimPathwayCode = 12,
                    LearningAimFrameworkCode = 1245,
                    LearningAimFundingLineType = "Non-DAS 16-18 Learner",
                    LearningAimStandardCode = 1209,
                    LearningAimProgrammeType = 7890,
                    LearningAimReference = "1234567-aim-ref",
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    TransactionType = TransactionType.Completion,
                    SfaContributionPercentage = 0.9m,
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    Amount = 300,
                    AccountId = 123456789,
                    TransferSenderAccountId = 123456789,
                    CompletionAmount = 3000,
                    CompletionStatus = 1,
                    InstalmentAmount = 100,
                    StartDate = DateTime.UtcNow,
                    ActualEndDate = DateTime.UtcNow,
                    NumberOfInstalments = 12,
                    PlannedEndDate = DateTime.UtcNow,
                    ApprenticeshipId = 800L,
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    ReportingAimFundingLineType = "ReportingAimFundingLineType",
                    ContractType = ContractType.Act1
                },
                new PaymentModel
                {
                    JobId = 123,
                    EventId = Guid.NewGuid(),
                    EventTime = DateTimeOffset.UtcNow,
                    Ukprn = 12345,
                    DeliveryPeriod = 12,
                    CollectionPeriod = new CollectionPeriod { Period = 2, AcademicYear = 1920 },
                    LearnerUln = 123456,
                    LearnerReferenceNumber = "1234-ref",
                    PriceEpisodeIdentifier = "pe-1",
                    LearningAimPathwayCode = 12,
                    LearningAimFrameworkCode = 1245,
                    LearningAimFundingLineType = "Non-DAS 16-18 Learner",
                    LearningAimStandardCode = 1209,
                    LearningAimProgrammeType = 7890,
                    LearningAimReference = "1234567-aim-ref",
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    TransactionType = TransactionType.Completion,
                    SfaContributionPercentage = 0.9m,
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    Amount = 300,
                    AccountId = 123456789,
                    TransferSenderAccountId = 123456789,
                    CompletionAmount = 3000,
                    CompletionStatus = 1,
                    InstalmentAmount = 100,
                    StartDate = DateTime.UtcNow,
                    ActualEndDate = DateTime.UtcNow,
                    NumberOfInstalments = 12,
                    PlannedEndDate = DateTime.UtcNow,
                    ApprenticeshipId = 800L,
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    ReportingAimFundingLineType = "ReportingAimFundingLineType",
                    ContractType = ContractType.Act1
                },
                new PaymentModel
                {
                    JobId = 123,
                    EventId = Guid.NewGuid(),
                    EventTime = DateTimeOffset.UtcNow,
                    Ukprn = 12345,
                    DeliveryPeriod = 12,
                    CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 1920 },
                    LearnerUln = 123456,
                    LearnerReferenceNumber = "1234-ref",
                    PriceEpisodeIdentifier = "pe-1",
                    LearningAimPathwayCode = 12,
                    LearningAimFrameworkCode = 1245,
                    LearningAimFundingLineType = "Non-DAS 16-18 Learner",
                    LearningAimStandardCode = 1209,
                    LearningAimProgrammeType = 7890,
                    LearningAimReference = "1234567-aim-ref",
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    TransactionType = TransactionType.Completion,
                    SfaContributionPercentage = 0.9m,
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    Amount = 300,
                    AccountId = 123456789,
                    TransferSenderAccountId = 123456789,
                    CompletionAmount = 3000,
                    CompletionStatus = 1,
                    InstalmentAmount = 100,
                    StartDate = DateTime.UtcNow,
                    ActualEndDate = DateTime.UtcNow,
                    NumberOfInstalments = 12,
                    PlannedEndDate = DateTime.UtcNow,
                    ApprenticeshipId = 800L,
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    ReportingAimFundingLineType = "ReportingAimFundingLineType",
                    ContractType = ContractType.Act2
                },
                new PaymentModel
                {
                    JobId = 123,
                    EventId = Guid.NewGuid(),
                    EventTime = DateTimeOffset.UtcNow,
                    Ukprn = 12345,
                    DeliveryPeriod = 12,
                    CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 1920 },
                    LearnerUln = 123456,
                    LearnerReferenceNumber = "1234-ref",
                    PriceEpisodeIdentifier = "pe-1",
                    LearningAimPathwayCode = 12,
                    LearningAimFrameworkCode = 1245,
                    LearningAimFundingLineType = "Non-DAS 16-18 Learner",
                    LearningAimStandardCode = 1209,
                    LearningAimProgrammeType = 7890,
                    LearningAimReference = "1234567-aim-ref",
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    TransactionType = TransactionType.Balancing,
                    SfaContributionPercentage = 0.9m,
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    Amount = 300,
                    AccountId = 123456789,
                    TransferSenderAccountId = 123456789,
                    CompletionAmount = 3000,
                    CompletionStatus = 1,
                    InstalmentAmount = 100,
                    StartDate = DateTime.UtcNow,
                    ActualEndDate = DateTime.UtcNow,
                    NumberOfInstalments = 12,
                    PlannedEndDate = DateTime.UtcNow,
                    ApprenticeshipId = 800L,
                    ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                    ReportingAimFundingLineType = "ReportingAimFundingLineType",
                    ContractType = ContractType.Act1
                }
            };
        }
    }
}