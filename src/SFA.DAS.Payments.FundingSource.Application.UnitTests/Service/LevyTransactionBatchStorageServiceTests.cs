using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

        [TestCase(2601)]
        [TestCase(2627)]
        [TestCase(1205)]
        public async Task WhenStoringLevyTransactions_AndSqlExceptionThrown_ThenSaveLevyTransactionsIndividuallyIsCalled(int sqlExceptionNumber)
        {
            fixture.With_RepositoryThrowingSqlException(sqlExceptionNumber);

            await fixture.StoreLevyTransactions();

            fixture.Verify_SaveLevyTransactionsIndividually_WasCalled_AndModelsMappedCorrectly();
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

        public LevyTransactionBatchStorageServiceFixture With_RepositoryThrowingSqlException(int number)
        {
            mockLevyTransactionRepository
                .Setup(x => x.SaveLevyTransactions(It.IsAny<List<LevyTransactionModel>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(SqlExceptionCreator.NewSqlException(number));
            return this;
        }

        public void Verify_SaveLevyTransactions_WasCalled_AndModelsMappedCorrectly()
        {
            mockLevyTransactionRepository.Verify(x =>
                x.SaveLevyTransactions(It.Is<List<LevyTransactionModel>>(y =>
                    y.Count == calculatedRequiredLevyAmounts.Count &&
                    y.TrueForAll(ValidateLevyTransactionModel)), It.IsAny<CancellationToken>()), Times.Once);

        }

        public void Verify_SaveLevyTransactionsIndividually_WasCalled_AndModelsMappedCorrectly()
        {
            mockLevyTransactionRepository.Verify(x =>
                x.SaveLevyTransactionsIndividually(It.Is<List<LevyTransactionModel>>(y =>
                    y.Count == calculatedRequiredLevyAmounts.Count &&
                    y.TrueForAll(ValidateLevyTransactionModel)), It.IsAny<CancellationToken>()), Times.Once);

        }

        private bool ValidateLevyTransactionModel(LevyTransactionModel levyTransactionModel)
        {
            return calculatedRequiredLevyAmounts.Any(calculatedRequiredLevyAmount =>
                calculatedRequiredLevyAmount.CollectionPeriod.Period == levyTransactionModel.CollectionPeriod &&
                calculatedRequiredLevyAmount.CollectionPeriod.AcademicYear == levyTransactionModel.AcademicYear &&
                calculatedRequiredLevyAmount.JobId == levyTransactionModel.JobId &&
                calculatedRequiredLevyAmount.Ukprn == levyTransactionModel.Ukprn &&
                calculatedRequiredLevyAmount.AmountDue == levyTransactionModel.Amount &&
                calculatedRequiredLevyAmount.EarningEventId == levyTransactionModel.EarningEventId &&
                calculatedRequiredLevyAmount.DeliveryPeriod == levyTransactionModel.DeliveryPeriod &&
                (calculatedRequiredLevyAmount.AccountId == levyTransactionModel.AccountId ||levyTransactionModel.AccountId == 0) &&
                calculatedRequiredLevyAmount.EventId == levyTransactionModel.RequiredPaymentEventId &&
                calculatedRequiredLevyAmount.TransferSenderAccountId == levyTransactionModel.TransferSenderAccountId &&
                calculatedRequiredLevyAmount.ToJson() == levyTransactionModel.MessagePayload &&
                calculatedRequiredLevyAmount.GetType().FullName == levyTransactionModel.MessageType &&
                calculatedRequiredLevyAmount.IlrSubmissionDateTime == levyTransactionModel.IlrSubmissionDateTime &&
                calculatedRequiredLevyAmount.CalculateFundingAccountId(false) ==
                levyTransactionModel.FundingAccountId &&
                calculatedRequiredLevyAmount.ApprenticeshipEmployerType ==
                levyTransactionModel.ApprenticeshipEmployerType &&
                calculatedRequiredLevyAmount.ApprenticeshipId == levyTransactionModel.ApprenticeshipId &&
                calculatedRequiredLevyAmount.Learner.Uln == levyTransactionModel.LearnerUln &&
                calculatedRequiredLevyAmount.Learner.ReferenceNumber == levyTransactionModel.LearnerReferenceNumber &&
                calculatedRequiredLevyAmount.LearningAim.FrameworkCode ==
                levyTransactionModel.LearningAimFrameworkCode &&
                calculatedRequiredLevyAmount.LearningAim.PathwayCode == levyTransactionModel.LearningAimPathwayCode &&
                calculatedRequiredLevyAmount.LearningAim.FundingLineType ==
                levyTransactionModel.LearningAimFundingLineType &&
                calculatedRequiredLevyAmount.LearningAim.ProgrammeType ==
                levyTransactionModel.LearningAimProgrammeType &&
                calculatedRequiredLevyAmount.LearningAim.Reference == levyTransactionModel.LearningAimReference &&
                calculatedRequiredLevyAmount.LearningAim.StandardCode == levyTransactionModel.LearningAimStandardCode &&
                calculatedRequiredLevyAmount.LearningStartDate == levyTransactionModel.LearningStartDate &&
                calculatedRequiredLevyAmount.SfaContributionPercentage ==
                levyTransactionModel.SfaContributionPercentage &&
                calculatedRequiredLevyAmount.TransactionType == levyTransactionModel.TransactionType);
        }

        private class SqlExceptionCreator
        {
            private static T Construct<T>(params object[] p)
            {
                var ctors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                return (T)ctors.First(ctor => ctor.GetParameters().Length == p.Length).Invoke(p);
            }

            internal static SqlException NewSqlException(int number = 1)
            {
                SqlErrorCollection collection = Construct<SqlErrorCollection>();
                SqlError error = Construct<SqlError>(number, (byte)2, (byte)3, "server name", "error message", "proc", 100, null);

                typeof(SqlErrorCollection)
                    .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(collection, new object[] { error });


                return typeof(SqlException)
                    .GetMethod("CreateException", BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        CallingConventions.ExplicitThis,
                        new[] { typeof(SqlErrorCollection), typeof(string) },
                        new ParameterModifier[] { })
                    .Invoke(null, new object[] { collection, "7.0.0" }) as SqlException;
            }
        }


    }
}