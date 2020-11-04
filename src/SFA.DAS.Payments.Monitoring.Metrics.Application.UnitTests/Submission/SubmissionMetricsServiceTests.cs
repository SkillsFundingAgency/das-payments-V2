using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionMetricsServiceTests
    {
        private AutoMock moqer;
        private List<TransactionTypeAmounts> dcEarnings;
        private List<TransactionTypeAmounts> dasEarnings;
        private List<TransactionTypeAmounts> requiredPayments;
        private DataLockTypeCounts dataLocks;
        private decimal totalDataLockedEarnings;

        [SetUp]
        public void SetUp()
        {
            moqer = AutoMock.GetLoose();
            dcEarnings = TestsHelper.DefaultDcEarnings;
            dasEarnings = TestsHelper.DefaultDasEarnings;
            dataLocks = TestsHelper.DefaultDataLockedEarnings;
            requiredPayments = TestsHelper.DefaultRequiredPayments;
            totalDataLockedEarnings = TestsHelper.DefaultDataLockedTotal;

            var dcMetricsDataContext = moqer.Mock<IDcMetricsDataContext>();
            dcMetricsDataContext.Setup(ctx => ctx.GetEarnings(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dcEarnings);

            moqer.Mock<IDcMetricsDataContextFactory>()
                .Setup(factory => factory.CreateContext(It.IsAny<short>()))
                .Returns(dcMetricsDataContext.Object);

            var mockSubmissionSummary = moqer.Mock<ISubmissionSummary>();
            mockSubmissionSummary.Setup(x => x.GetMetrics())
                .Returns(new SubmissionSummaryModel
                {
                    SubmissionMetrics = new ContractTypeAmountsVerbose(),
                    DasEarnings = new ContractTypeAmountsVerbose(),
                    DataLockMetrics = new List<DataLockCountsModel>(),
                    DcEarnings = new ContractTypeAmounts(),
                    EarningsMetrics = new List<EarningsModel>(),
                    HeldBackCompletionPayments = new ContractTypeAmounts(),
                    RequiredPayments = new ContractTypeAmounts(),
                    RequiredPaymentsMetrics = new List<RequiredPaymentsModel>(),
                    YearToDatePayments = new ContractTypeAmounts()
                });
            moqer.Mock<ISubmissionSummaryFactory>()
                .Setup(factory =>
                    factory.Create(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .Returns(mockSubmissionSummary.Object);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetDasEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dasEarnings);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetDasEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dasEarnings);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetDataLockedEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataLocks);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetDataLockedEarningsTotal(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.DefaultDataLockedTotal);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetAlreadyPaidDataLockedEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.AlreadyPaidDataLockedEarnings);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetRequiredPayments(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(requiredPayments);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetHeldBackCompletionPaymentsTotal(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.DefaultHeldBackCompletionPayments);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Setup(repo => repo.GetYearToDatePaymentsTotal(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.DefaultYearToDateAmounts);
        }

        [Test]
        public async Task Includes_Earnings_In_Metrics()
        {
            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummary>()
                .Verify(x => x.AddEarnings(It.Is<List<TransactionTypeAmounts>>(lst => lst == dcEarnings),
                    It.Is<List<TransactionTypeAmounts>>(lst => lst == dasEarnings)), Times.Once);
        }

        [Test]
        public async Task Includes_DataLocks_In_Metrics()
        {
            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummary>()
                .Verify(x => x.AddDataLockTypeCounts(It.Is<decimal>(total => total == totalDataLockedEarnings), It.Is<DataLockTypeCounts>(amounts => amounts == dataLocks), It.Is<decimal>(amount => amount == TestsHelper.AlreadyPaidDataLockedEarnings)), Times.Once);

        }

        [Test]
        public async Task Includes_Required_Payments_In_Metrics()
        {
            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummary>()
                .Verify(x => x.AddRequiredPayments(It.Is<List<TransactionTypeAmounts>>(amounts => amounts == requiredPayments)), Times.Once);

        }

        [Test]
        public async Task Includes_Held_Back_Completion_Payments_In_Metrics()
        {
            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummary>()
                .Verify(x => x.AddHeldBackCompletionPayments(It.Is<ContractTypeAmounts>(amounts => amounts.ContractType1 == TestsHelper.DefaultHeldBackCompletionPayments.ContractType1 &&
                                                                                                   amounts.ContractType2 == TestsHelper.DefaultHeldBackCompletionPayments.ContractType2)), Times.Once);
        }


        [Test]
        public async Task Includes_Year_To_Date_Payments_In_Metrics()
        {
            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummary>()
                .Verify(x => x.AddYearToDatePaymentTotals(It.Is<ContractTypeAmounts>(amounts => amounts.ContractType1 == TestsHelper.DefaultYearToDateAmounts.ContractType1 &&
                                                                                                   amounts.ContractType2 == TestsHelper.DefaultYearToDateAmounts.ContractType2)), Times.Once);
        }

        [Test]
        public async Task Saves_Submission_Metrics()
        {
            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummary>()
                .Verify(x => x.GetMetrics(), Times.Once);
            moqer.Mock<ISubmissionMetricsRepository>()
                .Verify(repo => repo.SaveSubmissionMetrics(It.IsAny<SubmissionSummaryModel>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task Calculates_RequiredPaymentsDasEarningsComparison_Correctly()
        {
            moqer.Provide<ISubmissionSummaryFactory>(new SubmissionSummaryFactory());

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ITelemetry>()
                 .Verify(t => t.TrackEvent(
                             It.Is<string>(s => s == "Finished Generating Submission Metrics"),
                             It.IsAny<Dictionary<string, string>>(),
                             It.Is<Dictionary<string, double>>(dictionary => dictionary.Contains(new KeyValuePair<string, double>("RequiredPaymentsDasEarningsPercentageComparison",  90.8)))));
        }

        [Test]
        public async Task Sends_Metrics_Telemetry()
        {
            moqer.Provide<ISubmissionSummaryFactory>(new SubmissionSummaryFactory());

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ITelemetry>()
                 .Verify(t => t.TrackEvent(
                             It.Is<string>(s => s == "Finished Generating Submission Metrics"),
                             It.IsAny<Dictionary<string, string>>(),
                             It.Is<Dictionary<string, double>>(dictionary => VerifySubmissionSummaryStats(dictionary))));
        }

        private static bool VerifySubmissionSummaryStats(IDictionary<string, double> dictionary)
        {
            var expectedStats = new Dictionary<string, double>
            {
                { "Percentage", 100 },
                { "ContractType1Percentage", 100 },
                { "ContractType2Percentage", 100 },
                
                { "DifferenceTotal", 0 },
                { "DifferenceContractType1", 0 },
                { "DifferenceContractType2", 0 },
                
                { "EarningsPercentage", 100 },
                { "EarningsPercentageContractType1", 100 },
                { "EarningsPercentageContractType2", 100 },
                
                { "EarningsDifferenceTotal", 0 },
                { "EarningsDifferenceContractType1", 0 },
                { "EarningsDifferenceContractType2", 0 },
                
                { "DasEarningsTotal", 65200 },
                { "DasEarningsContractType1Total", 32600 },
                { "DasEarningsContractType2Total", 32600 },
                
                { "DcEarningsTotal", 65200 },
                { "DcEarningsContractType1Total", 32600 },
                { "DcEarningsContractType2Total", 32600 },
                
                { "DataLockedEarningsAmount", 3000 },
                
                { "DataLockedEarningsTotal", 4000 },
                
                { "DataLockAmountAlreadyPaid", 1000 },
                
                { "HeldBackCompletionPayments", 3000 },
                { "HeldBackCompletionPaymentsContractType1", 2000 },
                { "HeldBackCompletionPaymentsContractType2", 2000 },
                
                { "YearToDatePaymentsTotal", 32600 },
                { "YearToDatePaymentsContractType1Total", 16300 },
                { "YearToDatePaymentsContractType2Total", 16300 },
                
                { "DasEarningsTransactionType1", 48000 },
                { "DasEarningsTransactionType2", 0 },
                { "DasEarningsTransactionType3", 12000 },
                { "DasEarningsTransactionType4", 400 },
                { "DasEarningsTransactionType5", 400 },
                { "DasEarningsTransactionType6", 400 },
                { "DasEarningsTransactionType7", 400 },
                { "DasEarningsTransactionType8", 400 },
                { "DasEarningsTransactionType9", 400 },
                { "DasEarningsTransactionType10", 400 },
                { "DasEarningsTransactionType11", 400 },
                { "DasEarningsTransactionType12", 400 },
                { "DasEarningsTransactionType13", 400 },
                { "DasEarningsTransactionType14", 400 },
                { "DasEarningsTransactionType15", 400 },
                { "DasEarningsTransactionType16", 400 },
                
                { "DcEarningsTransactionType1", 48000 },
                { "DcEarningsTransactionType2", 0 },
                { "DcEarningsTransactionType3", 12000 },
                { "DcEarningsTransactionType4", 400 },
                { "DcEarningsTransactionType5", 400 },
                { "DcEarningsTransactionType6", 400 },
                { "DcEarningsTransactionType7", 400 },
                { "DcEarningsTransactionType8", 400 },
                { "DcEarningsTransactionType9", 400 },
                { "DcEarningsTransactionType10", 400 },
                { "DcEarningsTransactionType11", 400 },
                { "DcEarningsTransactionType12", 400 },
                { "DcEarningsTransactionType13", 400 },
                { "DcEarningsTransactionType14", 400 },
                { "DcEarningsTransactionType15", 400 },
                { "DcEarningsTransactionType16", 400 },
                
                { "DataLockedCountDLock1", 0 },
                { "DataLockedCountDLock2", 1000 },
                { "DataLockedCountDLock3", 0 },
                { "DataLockedCountDLock4", 1000 },
                { "DataLockedCountDLock5", 0 },
                { "DataLockedCountDLock6", 0 },
                { "DataLockedCountDLock7", 2000 },
                { "DataLockedCountDLock8", 0 },
                { "DataLockedCountDLock9", 0 },
                { "DataLockedCountDLock10", 0 },
                { "DataLockedCountDLock11", 0 },
                { "DataLockedCountDLock12", 0 },
                
                { "RequiredPaymentsTotal", 26600 },
                { "RequiredPaymentsAct1Total", 11300 },
                { "RequiredPaymentsAc2Total", 15300 },
                
                { "RequiredPaymentsAct1TotalTransactionType1", 9000 },
                { "RequiredPaymentsAct1TotalTransactionType2", 0 },
                { "RequiredPaymentsAct1TotalTransactionType3", 1000 },
                { "RequiredPaymentsAct1TotalTransactionType4", 100 },
                { "RequiredPaymentsAct1TotalTransactionType5", 100 },
                { "RequiredPaymentsAct1TotalTransactionType6", 100 },
                { "RequiredPaymentsAct1TotalTransactionType7", 100 },
                { "RequiredPaymentsAct1TotalTransactionType8", 100 },
                { "RequiredPaymentsAct1TotalTransactionType9", 100 },
                { "RequiredPaymentsAct1TotalTransactionType10", 100 },
                { "RequiredPaymentsAct1TotalTransactionType11", 100 },
                { "RequiredPaymentsAct1TotalTransactionType12", 100 },
                { "RequiredPaymentsAct1TotalTransactionType13", 100 },
                { "RequiredPaymentsAct1TotalTransactionType14", 100 },
                { "RequiredPaymentsAct1TotalTransactionType15", 100 },
                { "RequiredPaymentsAct1TotalTransactionType16", 100 },
                
                { "RequiredPaymentsAct2TotalTransactionType1", 12000 },
                { "RequiredPaymentsAct2TotalTransactionType2", 0 },
                { "RequiredPaymentsAct2TotalTransactionType3", 2000 },
                { "RequiredPaymentsAct2TotalTransactionType4", 100 },
                { "RequiredPaymentsAct2TotalTransactionType5", 100 },
                { "RequiredPaymentsAct2TotalTransactionType6", 100 },
                { "RequiredPaymentsAct2TotalTransactionType7", 100 },
                { "RequiredPaymentsAct2TotalTransactionType8", 100 },
                { "RequiredPaymentsAct2TotalTransactionType9", 100 },
                { "RequiredPaymentsAct2TotalTransactionType10", 100 },
                { "RequiredPaymentsAct2TotalTransactionType11", 100 },
                { "RequiredPaymentsAct2TotalTransactionType12", 100 },
                { "RequiredPaymentsAct2TotalTransactionType13", 100 },
                { "RequiredPaymentsAct2TotalTransactionType14", 100 },
                { "RequiredPaymentsAct2TotalTransactionType15", 100 },
                { "RequiredPaymentsAct2TotalTransactionType16", 100 },
                
                { "RequiredPaymentsDasEarningsPercentageComparison", 90.8 },
            };

            foreach (var keyValue in expectedStats)
            {
                dictionary.Should().Contain(keyValue);
            }

            return true;
        }
    }
}