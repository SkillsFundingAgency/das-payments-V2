using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class LevyTransactionBatchStorageServiceTests
    {
        private LevyTransactionBatchStorageServiceFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new LevyTransactionBatchStorageServiceFixture();
        }

        [Test]
        public async Task WhenStoringLevyTransactions_ThenSaveLevyTransactionsIsCalled_AndModelsMappedCorrectly()
        {
            await fixture.StoreLevyTransactions();

            fixture.Verify_SaveLevyTransactions_WasCalled_AndModelsMappedCorrectly();
        }
    }

    internal class LevyTransactionBatchStorageServiceFixture
    {
        private readonly Mock<IPaymentLogger> mockLogger;
        private readonly Mock<ILevyTransactionRepository> mockLevyTransactionRepository;

        private readonly IList<CalculatedRequiredLevyAmount> calculatedRequiredLevyAmounts;

        private readonly LevyTransactionBatchStorageService sut;

        public LevyTransactionBatchStorageServiceFixture()
        {
            var fixture = new Fixture();

            mockLogger = new Mock<IPaymentLogger>();
            mockLevyTransactionRepository = new Mock<ILevyTransactionRepository>();

            calculatedRequiredLevyAmounts = fixture.Create<IList<CalculatedRequiredLevyAmount>>();

            sut = new LevyTransactionBatchStorageService(mockLogger.Object, mockLevyTransactionRepository.Object);
        }

        public Task StoreLevyTransactions() => sut.StoreLevyTransactions(calculatedRequiredLevyAmounts, It.IsAny<CancellationToken>());

        public void Verify_SaveLevyTransactions_WasCalled_AndModelsMappedCorrectly()
        {
            mockLevyTransactionRepository.Verify(x =>
                x.SaveLevyTransactions(It.Is<List<LevyTransactionModel>>(y =>
                    y.Count == calculatedRequiredLevyAmounts.Count &&
                    y.TrueForAll(levyTransactionModel => calculatedRequiredLevyAmounts.Any(calculatedRequiredLevyAmount =>
                        calculatedRequiredLevyAmount.CollectionPeriod.Period == levyTransactionModel.CollectionPeriod &&
                        calculatedRequiredLevyAmount.CollectionPeriod.AcademicYear == levyTransactionModel.AcademicYear &&
                        calculatedRequiredLevyAmount.JobId == levyTransactionModel.JobId &&
                        calculatedRequiredLevyAmount.Ukprn == levyTransactionModel.Ukprn &&
                        calculatedRequiredLevyAmount.AmountDue == levyTransactionModel.Amount &&
                        calculatedRequiredLevyAmount.EarningEventId == levyTransactionModel.EarningEventId &&
                        calculatedRequiredLevyAmount.DeliveryPeriod == levyTransactionModel.DeliveryPeriod &&
                        (calculatedRequiredLevyAmount.AccountId == levyTransactionModel.AccountId || levyTransactionModel.AccountId == 0) &&
                        calculatedRequiredLevyAmount.EventId == levyTransactionModel.RequiredPaymentEventId &&
                        calculatedRequiredLevyAmount.TransferSenderAccountId == levyTransactionModel.TransferSenderAccountId &&
                        calculatedRequiredLevyAmount.ToJson() == levyTransactionModel.MessagePayload &&
                        calculatedRequiredLevyAmount.GetType().FullName == levyTransactionModel.MessageType &&
                        calculatedRequiredLevyAmount.IlrSubmissionDateTime == levyTransactionModel.IlrSubmissionDateTime &&
                        calculatedRequiredLevyAmount.CalculateFundingAccountId(false) == levyTransactionModel.FundingAccountId &&
                        calculatedRequiredLevyAmount.ApprenticeshipEmployerType == levyTransactionModel.ApprenticeshipEmployerType &&
                        calculatedRequiredLevyAmount.ApprenticeshipId == levyTransactionModel.ApprenticeshipId &&
                        calculatedRequiredLevyAmount.Learner.ReferenceNumber == levyTransactionModel.LearnerReferenceNumber &&
                        calculatedRequiredLevyAmount.LearningAim.FrameworkCode == levyTransactionModel.LearningAimFrameworkCode &&
                        calculatedRequiredLevyAmount.LearningAim.PathwayCode == levyTransactionModel.LearningAimPathwayCode &&
                        calculatedRequiredLevyAmount.LearningAim.FundingLineType == levyTransactionModel.LearningAimFundingLineType &&
                        calculatedRequiredLevyAmount.LearningAim.ProgrammeType == levyTransactionModel.LearningAimProgrammeType &&
                        calculatedRequiredLevyAmount.LearningAim.Reference == levyTransactionModel.LearningAimReference &&
                        calculatedRequiredLevyAmount.LearningAim.StandardCode == levyTransactionModel.LearningAimStandardCode &&
                        calculatedRequiredLevyAmount.LearningStartDate == levyTransactionModel.LearningStartDate &&
                        calculatedRequiredLevyAmount.SfaContributionPercentage == levyTransactionModel.SfaContributionPercentage &&
                        calculatedRequiredLevyAmount.TransactionType == levyTransactionModel.TransactionType
                    ))), It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}