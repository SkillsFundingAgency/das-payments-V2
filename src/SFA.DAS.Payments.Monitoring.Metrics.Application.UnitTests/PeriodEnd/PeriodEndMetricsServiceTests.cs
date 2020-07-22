using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.PeriodEnd
{
    [TestFixture]
    public class PeriodEndMetricsServiceTests
    {
        private Autofac.Extras.Moq.AutoMock moqer;

        private Mock<IPeriodEndProviderSummary> periodEndProviderSummary;
        private Mock<IPeriodEndSummary> periodEndSummary;
        private Mock<IDcMetricsDataContext> dcMetricsDataContextMock;
        private Mock<IPeriodEndMetricsRepository> periodEndMetricsRepositoryMock;

        private List<ProviderTransactionTypeAmounts> DcEarnings = PeriodEndTestHelper.SingleProviderDcEarnings;

        [SetUp]
        public void SetUp()
        {
            moqer = AutoMock.GetLoose();

            periodEndSummary = moqer.Mock<IPeriodEndSummary>();
            periodEndProviderSummary = moqer.Mock<IPeriodEndProviderSummary>();

            periodEndSummary
                .Setup(x => x.GetMetrics())
                .Returns(new PeriodEndSummaryModel());

            periodEndSummary
                .Setup(x => x.AddProviderSummaries(It.IsAny<List<ProviderPeriodEndSummaryModel>>()));

            periodEndProviderSummary
                .Setup(x => x.GetMetrics())
                .Returns(new ProviderPeriodEndSummaryModel());

            moqer.Mock<IPeriodEndSummaryFactory>()
                .Setup(x => x.CreatePeriodEndSummary(It.IsAny<long>(), It.IsAny<byte>(), It.IsAny<short>()))
                .Returns(periodEndSummary.Object);
            moqer.Mock<IPeriodEndSummaryFactory>()
                .Setup(x => x.CreatePeriodEndProviderSummary(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<byte>(), It.IsAny<short>()))
                .Returns(periodEndProviderSummary.Object);

            dcMetricsDataContextMock = moqer.Mock<IDcMetricsDataContext>();
            dcMetricsDataContextMock
                .Setup(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(DcEarnings);

            periodEndMetricsRepositoryMock = moqer.Mock<IPeriodEndMetricsRepository>();
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetTransactionTypesByContractType(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderTransactionTypeAmounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetFundingSourceAmountsByContractType(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderFundingSourceAmounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetYearToDatePayments(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderContractTypeAmounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetDataLockedEarningsTotals(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderTotal>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetAlreadyPaidDataLockedEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderTotal>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetHeldBackCompletionPaymentsTotals(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderContractTypeAmounts>());
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenDcEarningsDataIsRetrieved()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            dcMetricsDataContextMock
                .Verify(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenDasDataIsRetrieved()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndMetricsRepositoryMock
                .Verify(
                    x => x.GetTransactionTypesByContractType(It.IsAny<short>(), It.IsAny<byte>(),
                        It.IsAny<CancellationToken>()), Times.Once);

            periodEndMetricsRepositoryMock
                .Verify(x =>
                    x.GetFundingSourceAmountsByContractType(It.IsAny<short>(), It.IsAny<byte>(),
                        It.IsAny<CancellationToken>()));

            periodEndMetricsRepositoryMock
                .Verify(x =>
                    x.GetYearToDatePayments(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()));

            periodEndMetricsRepositoryMock
                .Verify(x =>
                    x.GetDataLockedEarningsTotals(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()));

            periodEndMetricsRepositoryMock
                .Verify(x =>
                    x.GetAlreadyPaidDataLockedEarnings(It.IsAny<short>(), It.IsAny<byte>(),
                        It.IsAny<CancellationToken>()));

            periodEndMetricsRepositoryMock
                .Verify(x =>
                    x.GetHeldBackCompletionPaymentsTotals(It.IsAny<short>(), It.IsAny<byte>(),
                        It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenReturnedDataIsAddedToProvider_ForEachProvider()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndProviderSummary.Verify(x => x.AddDcEarnings(It.IsAny<IEnumerable<TransactionTypeAmountsByContractType>>()), Times.Once);
            periodEndProviderSummary.Verify(x => x.AddTransactionTypes(It.IsAny<IEnumerable<TransactionTypeAmountsByContractType>>()), Times.Once);
            periodEndProviderSummary.Verify(x => x.AddFundingSourceAmounts(It.IsAny<IEnumerable<ProviderFundingSourceAmounts>>()), Times.Once);
            periodEndProviderSummary.Verify(x => x.AddPaymentsYearToDate(It.IsAny<ProviderContractTypeAmounts>()), Times.Once);
            periodEndProviderSummary.Verify(x => x.AddDataLockedEarnings(It.IsAny<decimal>()), Times.Once);
            periodEndProviderSummary.Verify(x => x.AddDataLockedAlreadyPaid(It.IsAny<decimal>()), Times.Once);
            periodEndProviderSummary.Verify(x => x.AddHeldBackCompletionPayments(It.IsAny<ProviderContractTypeAmounts>()), Times.Once);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public async Task WhenBuildingMetrics_ThenGetMetricsIsCalled_ForEachProviderSummary(int numberOfProviders)
        {
            dcMetricsDataContextMock
                .Setup(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(PeriodEndTestHelper.MultipleProviderDcEarnings(numberOfProviders));

            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndProviderSummary.Verify(x => x.GetMetrics(), Times.Exactly(numberOfProviders));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public async Task WhenBuildingMetrics_ThenProviderSummariesAreAddedToPeriodEndSummary_AndCallsGetMetrics(int numberOfProviders)
        {
            dcMetricsDataContextMock
                .Setup(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(PeriodEndTestHelper.MultipleProviderDcEarnings(numberOfProviders));

            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndSummary.Verify(x => x.AddProviderSummaries(It.Is<List<ProviderPeriodEndSummaryModel>>(y => y.Count == numberOfProviders)));
            periodEndSummary.Verify(x => x.GetMetrics(), Times.Once);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenProviderSummariesSaved()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndMetricsRepositoryMock
                .Verify(x => x.SaveProviderSummaries(It.IsAny<List<ProviderPeriodEndSummaryModel>>(), It.IsAny<PeriodEndSummaryModel>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}