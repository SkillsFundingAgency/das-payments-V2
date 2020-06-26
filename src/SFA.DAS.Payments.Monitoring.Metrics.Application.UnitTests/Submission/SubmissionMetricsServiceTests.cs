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
            moqer.Mock<IDcMetricsDataContext>()
                .Setup(ctx => ctx.GetEarnings(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dcEarnings);
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
        public async Task Calculates_RequiredPaymentsDasEarningsDifference_Correctly()
        {
            moqer.Provide<ISubmissionSummaryFactory>(new SubmissionSummaryFactory());

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ITelemetry>()
                 .Verify(t => t.TrackEvent(
                             It.Is<string>(s => s == "Finished Generating Submission Metrics"),
                             It.IsAny<Dictionary<string, string>>(),
                             It.Is<Dictionary<string, double>>(dictionary => dictionary.Contains(new KeyValuePair<string, double>("RequiredPaymentsDasEarningsPercentageDifference",  81.6)))));
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
                { "Percentage",  200 },
                { "ContractType1Percentage",  200 },
                { "ContractType2Percentage",  200 },
                { "EarningsContractType1Percentage",  200 },
                { "EarningsContractType2Percentage",  200 },
                { "DcEarningsTotal",  32600 },
                { "DasEarningsTotal",  32600 },
                { "DasEarningsTransactionType1",  48000 },
                { "DasEarningsTransactionType2",  0 },
                { "DasEarningsTransactionType3",  12000 },
                { "DasEarningsTransactionType4",  400 },
                { "DasEarningsTransactionType5",  400 },
                { "DasEarningsTransactionType6",  400 },
                { "DasEarningsTransactionType7",  400 },
                { "DasEarningsTransactionType8",  400 },
                { "DasEarningsTransactionType9",  400 },
                { "DasEarningsTransactionType10",  400 },
                { "DasEarningsTransactionType11",  400 },
                { "DasEarningsTransactionType12",  400 },
                { "DasEarningsTransactionType13",  400 },
                { "DasEarningsTransactionType14",  400 },
                { "DasEarningsTransactionType15",  400 },
                { "DasEarningsTransactionType16",  400 },
                { "DataLockedEarningsAmount",  3000 },
                { "DataLockedCountDLock1",  0 },
                { "DataLockedCountDLock2",  1000 },
                { "DataLockedCountDLock3",  0 },
                { "DataLockedCountDLock4",  1000 },
                { "DataLockedCountDLock5",  0 },
                { "DataLockedCountDLock6",  0 },
                { "DataLockedCountDLock7",  2000 },
                { "DataLockedCountDLock8",  0 },
                { "DataLockedCountDLock9",  0 },
                { "DataLockedCountDLock10",  0 },
                { "DataLockedCountDLock11",  0 },
                { "DataLockedCountDLock12",  0 },
                { "DataLockAmountAlreadyPaid",  1000 },
                { "HeldBackCompletionPayments",  3000 },
                { "RequiredPaymentsTotal",  26600 },
                { "RequiredPaymentsTotalTransactionType1",  21000 },
                { "RequiredPaymentsTotalTransactionType2",  0 },
                { "RequiredPaymentsTotalTransactionType3",  3000 },
                { "RequiredPaymentsTotalTransactionType4",  200 },
                { "RequiredPaymentsTotalTransactionType5",  200 },
                { "RequiredPaymentsTotalTransactionType6",  200 },
                { "RequiredPaymentsTotalTransactionType7",  200 },
                { "RequiredPaymentsTotalTransactionType8",  200 },
                { "RequiredPaymentsTotalTransactionType9",  200 },
                { "RequiredPaymentsTotalTransactionType10",  200 },
                { "RequiredPaymentsTotalTransactionType11",  200 },
                { "RequiredPaymentsTotalTransactionType12",  200 },
                { "RequiredPaymentsTotalTransactionType13",  200 },
                { "RequiredPaymentsTotalTransactionType14",  200 },
                { "RequiredPaymentsTotalTransactionType15",  200 },
                { "RequiredPaymentsTotalTransactionType16",  200 },
                { "RequiredPaymentsDasEarningsPercentageDifference",  81.6 }
            };

            foreach (var keyValue in expectedStats)
            {
                dictionary.Should().Contain(keyValue);
            }

            return true;
        }
    }
}