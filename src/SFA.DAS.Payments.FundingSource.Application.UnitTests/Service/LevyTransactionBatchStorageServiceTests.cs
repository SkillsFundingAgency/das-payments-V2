using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Data;
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
        public async Task WhenStoringLevyTransactions_AndSaveLevyTransactionsIsCalled()
        {
            await fixture.StoreLevyTransactions();

            fixture.Verify_SaveLevyTransactions_WasCalled();
        }
    }

    internal class LevyTransactionBatchStorageServiceFixture
    {
        private readonly Mock<IPaymentLogger> mockLogger;
        private readonly Mock<IFundingSourceDataContextFactory> mockFundingSourceDataContextFactory;
        private readonly Mock<ILevyTransactionRepository> mockLevyTransactionRepository;

        private readonly IList<CalculatedRequiredLevyAmount> calculatedRequiredLevyAmounts;

        private readonly LevyTransactionBatchStorageService sut;

        public LevyTransactionBatchStorageServiceFixture()
        {
            var fixture = new Fixture();

            mockLogger = new Mock<IPaymentLogger>();
            mockFundingSourceDataContextFactory = new Mock<IFundingSourceDataContextFactory>();
            mockLevyTransactionRepository = new Mock<ILevyTransactionRepository>();

            calculatedRequiredLevyAmounts = fixture.Create<IList<CalculatedRequiredLevyAmount>>();

            sut = new LevyTransactionBatchStorageService(mockLogger.Object, mockFundingSourceDataContextFactory.Object, mockLevyTransactionRepository.Object);
        }

        public Task StoreLevyTransactions() => sut.StoreLevyTransactions(calculatedRequiredLevyAmounts, It.IsAny<CancellationToken>());

        public void Verify_SaveLevyTransactions_WasCalled()
        {
            mockLevyTransactionRepository.Verify(x => x.SaveLevyTransactions(It.IsAny<List<LevyTransactionModel>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}