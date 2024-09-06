using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
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

        [Test]
        public async Task WhenStoringLevyTransactionsFromDasPaymentPlatform_ThenSaveLevyTransactionsIsCalled_AndModelsMappedCorrectly()
        {
            await fixture.StoreLevyTransactionsFromDasPaymentPlatform();

//            fixture.Verify_SaveLevyTransactionsIndividually_WasCalled_AndModelsMappedCorrectly();
            fixture.Verify_SaveLevyTransactions_FromDasPaymentPlatform_WasCalled_AndModelsMappedCorrectly();
        }
    }

    internal class LevyTransactionBatchStorageServiceFixture
    {
        private readonly Mock<IPaymentLogger> mockLogger;
        private readonly Mock<ILevyTransactionRepository> mockLevyTransactionRepository;

        private readonly IList<CalculatedRequiredLevyAmount> calculatedRequiredLevyAmounts;

        private readonly IList<CalculateOnProgrammePayment> calculatedOnProgrammePayments;

        private readonly LevyTransactionBatchStorageService sut;
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;


        public LevyTransactionBatchStorageServiceFixture()
        {
            var fixture = new Fixture();

            mockLogger = new Mock<IPaymentLogger>();
            mockLevyTransactionRepository = new Mock<ILevyTransactionRepository>();

            calculatedRequiredLevyAmounts = fixture.Create<IList<CalculatedRequiredLevyAmount>>();

            calculatedOnProgrammePayments = fixture.Create<IList<CalculateOnProgrammePayment>>();
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
            sut = new LevyTransactionBatchStorageService(mockLogger.Object, mockLevyTransactionRepository.Object, autoMapper);
        }

        public Task StoreLevyTransactions() => sut.StoreLevyTransactions(calculatedRequiredLevyAmounts, It.IsAny<CancellationToken>());

        public Task StoreLevyTransactionsFromDasPaymentPlatform() => sut.StoreLevyTransactions(calculatedOnProgrammePayments, It.IsAny<CancellationToken>());

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

        public void Verify_SaveLevyTransactions_FromDasPaymentPlatform_WasCalled_AndModelsMappedCorrectly()
        {
            mockLevyTransactionRepository.Verify(x =>
                x.SaveLevyTransactions(It.Is<List<LevyTransactionModel>>(y =>
                    y.Count == calculatedRequiredLevyAmounts.Count &&
                    y.TrueForAll(ValidateDasPaymentPlatformLevyTransactionModel)), It.IsAny<CancellationToken>()), Times.Once);
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
                (calculatedRequiredLevyAmount.AccountId == levyTransactionModel.AccountId ||
                 levyTransactionModel.AccountId == 0) &&
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
                calculatedRequiredLevyAmount.TransactionType == levyTransactionModel.TransactionType &&
                levyTransactionModel.FundingPlatformType == FundingPlatformType.SubmitLearnerData);
        }

        private bool ValidateDasPaymentPlatformLevyTransactionModel(LevyTransactionModel levyTransactionModel)
        {
            return calculatedOnProgrammePayments.Any(calculatedOnProgrammePayment =>
                levyTransactionModel.CollectionPeriod == calculatedOnProgrammePayment.CollectionPeriod.Period &&
                levyTransactionModel.AcademicYear == calculatedOnProgrammePayment.CollectionPeriod.AcademicYear &&
                levyTransactionModel.JobId == -1 &&
                levyTransactionModel.Ukprn == calculatedOnProgrammePayment.Ukprn &&
                levyTransactionModel.Amount == calculatedOnProgrammePayment.AmountDue &&
                levyTransactionModel.EarningEventId == Guid.Empty &&
                levyTransactionModel.DeliveryPeriod == calculatedOnProgrammePayment.DeliveryPeriod &&
                (levyTransactionModel.AccountId == calculatedOnProgrammePayment.AccountId || levyTransactionModel.AccountId == 0) &&
                levyTransactionModel.RequiredPaymentEventId == calculatedOnProgrammePayment.EventId &&
                levyTransactionModel.ClawbackSourcePaymentEventId == null &&
                levyTransactionModel.TransferSenderAccountId == calculatedOnProgrammePayment.TransferSenderAccountId &&
                //TODO: internally it now temporarily maps to the calculated required payment amount 
                //levyTransactionModel.MessagePayload == calculatedOnProgrammePayment.ToJson() &&
                //levyTransactionModel.MessageType == calculatedOnProgrammePayment.GetType().FullName &&
                levyTransactionModel.IlrSubmissionDateTime == new DateTime(1753, 1, 1) &&
                //levyTransactionModel.FundingAccountId == calculatedOnProgrammePayment.CalculateFundingAccountId(false) &&
                levyTransactionModel.ApprenticeshipEmployerType == calculatedOnProgrammePayment.ApprenticeshipEmployerType &&
                levyTransactionModel.ApprenticeshipId == calculatedOnProgrammePayment.ApprenticeshipId &&
                levyTransactionModel.LearnerUln == calculatedOnProgrammePayment.Learner.Uln &&
                levyTransactionModel.LearnerReferenceNumber == calculatedOnProgrammePayment.Learner.ReferenceNumber &&
                levyTransactionModel.LearningAimFrameworkCode == calculatedOnProgrammePayment.LearningAim.FrameworkCode &&
                levyTransactionModel.LearningAimPathwayCode == calculatedOnProgrammePayment.LearningAim.PathwayCode &&
                levyTransactionModel.LearningAimFundingLineType == calculatedOnProgrammePayment.LearningAim.FundingLineType &&
                levyTransactionModel.LearningAimProgrammeType == calculatedOnProgrammePayment.LearningAim.ProgrammeType &&
                levyTransactionModel.LearningAimReference == calculatedOnProgrammePayment.LearningAim.Reference &&
                levyTransactionModel.LearningAimStandardCode == calculatedOnProgrammePayment.LearningAim.StandardCode &&
                levyTransactionModel.LearningStartDate == calculatedOnProgrammePayment.LearningStartDate &&
                levyTransactionModel.SfaContributionPercentage == calculatedOnProgrammePayment.SfaContributionPercentage &&
                levyTransactionModel.TransactionType == (TransactionType)calculatedOnProgrammePayment.OnProgrammeEarningType &&
                levyTransactionModel.FundingPlatformType == calculatedOnProgrammePayment.FundingPlatformType);
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