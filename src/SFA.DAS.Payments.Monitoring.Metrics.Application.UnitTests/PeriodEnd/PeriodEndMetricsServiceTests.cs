using System;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.PeriodEnd
{
    [TestFixture]
    public class PeriodEndMetricsServiceTests
    {
        private AutoMock moqer;

        private Mock<IPeriodEndProviderSummary> periodEndProviderSummary;
        private Mock<IPeriodEndSummary> periodEndSummary;
        private Mock<IDcMetricsDataContext> dcMetricsDataContextMock;
        private Mock<IPeriodEndMetricsRepository> periodEndMetricsRepositoryMock;
        private Mock<ITelemetry> telemetryMock;
        private Mock<IPeriodEndSummaryFactory> periodEndSummaryFactory;
        private List<ProviderLearnerNegativeEarningsTotal> dcNegativeEarningsResult;

        private CollectionPeriodToleranceModel collectionPeriodTolerance;

        private readonly List<ProviderTransactionTypeAmounts> dcEarnings = PeriodEndTestHelper.SingleProviderDcEarnings;

        [SetUp]
        public void SetUp()
        {
            moqer = AutoMock.GetLoose();
            var fixture = new Fixture();
            var random = new Random();


            dcNegativeEarningsResult = fixture.CreateMany<ProviderLearnerNegativeEarningsTotal>(10).ToList();
            dcNegativeEarningsResult.ForEach(x =>
            {
                var randomInt = random.Next(0, 2);

                x.ContractType = randomInt == 1 ? ContractType.Act1 : ContractType.Act2;
                x.NegativeEarningsTotal = Math.Abs(x.NegativeEarningsTotal);
            });

            periodEndSummary = moqer.Mock<IPeriodEndSummary>();
            periodEndProviderSummary = moqer.Mock<IPeriodEndProviderSummary>();
            periodEndSummaryFactory = moqer.Mock<IPeriodEndSummaryFactory>();
            collectionPeriodTolerance = new CollectionPeriodToleranceModel();
            telemetryMock = moqer.Mock<ITelemetry>();
            

            periodEndSummary
                .Setup(x => x.GetMetrics())
                .Returns(new PeriodEndSummaryModel());

            periodEndSummary
                .Setup(x => x.AddProviderSummaries(It.IsAny<List<ProviderPeriodEndSummaryModel>>()));

            periodEndSummaryFactory
                .Setup(x => x.CreatePeriodEndProviderSummary(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<byte>(), It.IsAny<short>()))
                .Returns(new PeriodEndProviderSummary(1, 1, 1, 1));

            periodEndSummary.Setup(x => x.GetMetrics())
                .Returns(new PeriodEndSummaryModel());

            periodEndProviderSummary
                .Setup(x => x.GetMetrics())
                .Returns(new ProviderPeriodEndSummaryModel());

            periodEndSummaryFactory
                .Setup(x => x.CreatePeriodEndSummary(It.IsAny<long>(), It.IsAny<byte>(), It.IsAny<short>()))
                .Returns(periodEndSummary.Object);

            dcMetricsDataContextMock = moqer.Mock<IDcMetricsDataContext>();
            dcMetricsDataContextMock
                .Setup(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dcEarnings);            
            
            dcMetricsDataContextMock
                .Setup(x => x.GetNegativeEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dcNegativeEarningsResult);

            var dcMetricsDataContextFactoryMock = moqer.Mock<IDcMetricsDataContextFactory>();
            dcMetricsDataContextFactoryMock
                .Setup(x => x.CreateContext(It.IsAny<short>()))
                .Returns(dcMetricsDataContextMock.Object);

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
                .ReturnsAsync(new List<ProviderFundingLineTypeAmounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetAlreadyPaidDataLockedEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderFundingLineTypeAmounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetHeldBackCompletionPaymentsTotals(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderContractTypeAmounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetPeriodEndProviderDataLockTypeCounts(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PeriodEndProviderDataLockTypeCounts>());
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetInLearningCount(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProviderInLearningTotal> { new ProviderInLearningTotal { InLearningCount = 0, Ukprn = dcEarnings.Select(x => x.Ukprn).First() } });
            periodEndMetricsRepositoryMock
                .Setup(x => x.GetCollectionPeriodTolerance(It.IsAny<byte>(), It.IsAny<short>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collectionPeriodTolerance);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenDcEarningsDataIsRetrieved()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            dcMetricsDataContextMock
                .Verify(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Test,AutoData]
        public async Task WhenBuildingMetrics_ThenDcNegativeEarningsDataIsRetrieved(long jobId, short academicYear, byte collectionPeriod)
        {
            //Arrange
            var service = moqer.Create<PeriodEndMetricsService>();

            //Act
            await service.BuildMetrics(jobId, academicYear, collectionPeriod, CancellationToken.None).ConfigureAwait(false);

            //Assert
            dcMetricsDataContextMock
                .Verify(x => x.GetNegativeEarnings(academicYear, collectionPeriod, It.IsAny<CancellationToken>()), Times.Once);
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

            periodEndMetricsRepositoryMock
                .Verify(x => x.GetPeriodEndProviderDataLockTypeCounts(It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()), Times.Once);

            periodEndMetricsRepositoryMock
                .Verify(x => x.GetInLearningCount(It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenReturnedDataIsAddedToProvider_ForEachProvider()
        {
            var periodEndSummary = new PeriodEndProviderSummaryFake();

            periodEndSummaryFactory
                .Setup(x => x.CreatePeriodEndProviderSummary(It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<byte>(), It.IsAny<short>()))
                .Returns(periodEndSummary);

            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);
            periodEndSummary.AddDcEarningsCalled.Should().BeTrue();
            periodEndSummary.AddTransactionTypesCalled.Should().BeTrue();
            periodEndSummary.AddFundingSourceAmountsCalled.Should().BeTrue();
            periodEndSummary.AddPaymentsYearToDateCalled.Should().BeTrue();
            periodEndSummary.AddDataLockedEarningsCalled.Should().BeTrue();
            periodEndSummary.AddDataLockedAlreadyPaidCalled.Should().BeTrue();
            periodEndSummary.AddHeldBackCompletionPaymentsCalled.Should().BeTrue();
            periodEndSummary.AddPeriodEndProviderDataLockTypeCountsCalled.Should().BeTrue();
            periodEndSummary.AddInLearningCountCalled.Should().BeTrue();
            periodEndSummary.AddNegativeEarningsCalled.Should().BeTrue();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public async Task WhenBuildingMetrics_ThenGetMetricsIsCalled_ForEachProviderSummary(int numberOfProviders)
        {
            periodEndSummaryFactory
                .Setup(x => x.CreatePeriodEndProviderSummary(It.IsAny<long>(), It.IsAny<long>(),
                    It.IsAny<byte>(), It.IsAny<short>()))
                .Returns(new PeriodEndProviderSummary(1, 1, 1, 1));

            var earnings = PeriodEndTestHelper.MultipleProviderDcEarnings(numberOfProviders);

            periodEndMetricsRepositoryMock
                .Setup(x => x.GetInLearningCount(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(earnings.Select(x => new ProviderInLearningTotal{ InLearningCount = 0, Ukprn = x.Ukprn }).ToList());

            dcMetricsDataContextMock
                .Setup(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(earnings);



            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndSummaryFactory.Verify(x => x.CreatePeriodEndProviderSummary(
                It.IsAny<long>(), It.IsAny<long>(),
                It.IsAny<byte>(), It.IsAny<short>()), Times.Exactly(numberOfProviders));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public async Task WhenBuildingMetrics_ThenProviderSummariesAreAddedToPeriodEndSummary_AndCallsGetMetrics(int numberOfProviders)
        {
            var earnings = PeriodEndTestHelper.MultipleProviderDcEarnings(numberOfProviders);

            periodEndMetricsRepositoryMock
                .Setup(x => x.GetInLearningCount(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(earnings.Select(x => new ProviderInLearningTotal { InLearningCount = 0, Ukprn = x.Ukprn }).ToList());

            dcMetricsDataContextMock
                .Setup(x => x.GetEarnings(It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(earnings);

            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndSummary.Verify(x => x.AddProviderSummaries(It.Is<List<ProviderPeriodEndSummaryModel>>(y => y.Count == numberOfProviders)));
            periodEndSummary.Verify(x => x.GetMetrics(), Times.Once);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenGetsCollectionPeriodTolerance()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndMetricsRepositoryMock.Verify(x => x.GetCollectionPeriodTolerance(1, 1920, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenCallsCalculateIsWithinTolerance()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndSummary.Verify(x => x.CalculateIsWithinTolerance(collectionPeriodTolerance.PeriodEndToleranceLower, collectionPeriodTolerance.PeriodEndToleranceUpper));
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenProviderSummariesSaved()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            periodEndMetricsRepositoryMock
                .Verify(x => x.SaveProviderSummaries(It.IsAny<List<ProviderPeriodEndSummaryModel>>(), It.IsAny<PeriodEndSummaryModel>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenTheDataIsSentToTelemetryService()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            Dictionary<string, string> properties = null;
            Dictionary<string, double> stats = null;

            telemetryMock.Setup(x => x.TrackEvent("Finished Generating Period End Metrics",
                    It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, double>>()))
                .Callback<string, Dictionary<string, string>, Dictionary<string, double>>((_, p, s) =>
                {
                    properties = p;
                    stats = s;
                });

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None);

            properties.Keys.Should().Contain("JobId");
            properties.Keys.Should().Contain("CollectionPeriod");
            properties.Keys.Should().Contain("AcademicYear");

            stats.Keys.Should().Contain("Percentage");
            stats.Keys.Should().Contain("ContractType1");
            stats.Keys.Should().Contain("ContractType2");
            stats.Keys.Should().Contain("DifferenceContractType1");
            stats.Keys.Should().Contain("DifferenceContractType2");
            stats.Keys.Should().Contain("PercentageContractType1");
            stats.Keys.Should().Contain("PercentageContractType2");
            stats.Keys.Should().Contain("EarningsDCContractType1");
            stats.Keys.Should().Contain("EarningsDCContractType2");
            stats.Keys.Should().Contain("PaymentsContractType1");
            stats.Keys.Should().Contain("PaymentsContractType2");
            stats.Keys.Should().Contain("DataLockedEarnings");
            stats.Keys.Should().Contain("DataLockedEarnings16To18");
            stats.Keys.Should().Contain("DataLockedEarnings19Plus");
            stats.Keys.Should().Contain("AlreadyPaidDataLockedEarnings");
            stats.Keys.Should().Contain("AlreadyPaidDataLockedEarnings16To18");
            stats.Keys.Should().Contain("AlreadyPaidDataLockedEarnings19Plus");
            stats.Keys.Should().Contain("TotalDataLockedEarnings");
            stats.Keys.Should().Contain("TotalDataLockedEarnings16To18");
            stats.Keys.Should().Contain("TotalDataLockedEarnings19Plus");
            stats.Keys.Should().Contain("HeldBackCompletionPaymentsContractType1");
            stats.Keys.Should().Contain("HeldBackCompletionPaymentsContractType2");
            stats.Keys.Should().Contain("PaymentsYearToDateContractType1");
            stats.Keys.Should().Contain("PaymentsYearToDateContractType2");

            stats.Keys.Should().Contain("DifferenceTotal");
            stats.Keys.Should().Contain("PercentageTotal");
            stats.Keys.Should().Contain("EarningsDCTotal");
            stats.Keys.Should().Contain("PaymentsTotal");
            stats.Keys.Should().Contain("HeldBackCompletionPaymentsTotal");
            stats.Keys.Should().Contain("PaymentsYearToDateTotal");

            stats.Keys.Should().Contain("DataLockedCountDLock1");
            stats.Keys.Should().Contain("DataLockedCountDLock2");
            stats.Keys.Should().Contain("DataLockedCountDLock3");
            stats.Keys.Should().Contain("DataLockedCountDLock4");
            stats.Keys.Should().Contain("DataLockedCountDLock5");
            stats.Keys.Should().Contain("DataLockedCountDLock6");
            stats.Keys.Should().Contain("DataLockedCountDLock7");
            stats.Keys.Should().Contain("DataLockedCountDLock8");
            stats.Keys.Should().Contain("DataLockedCountDLock9");
            stats.Keys.Should().Contain("DataLockedCountDLock10");
            stats.Keys.Should().Contain("DataLockedCountDLock11");
            stats.Keys.Should().Contain("DataLockedCountDLock12");

            stats.Keys.Should().Contain("InLearning");
        }

        [Test]
        public async Task WhenBuildingMetrics_ThenTheDataIsSentToTelemetryServiceForEachProvider()
        {
            var service = moqer.Create<PeriodEndMetricsService>();

            Dictionary<string, string> properties = null;
            Dictionary<string, double> stats = null;

            telemetryMock.Setup(x => x.TrackEvent("Finished Generating Period End Metrics for Provider: 1",
                    It.IsAny<Dictionary<string, string>>(), It.IsAny<Dictionary<string, double>>()))
                .Callback<string, Dictionary<string, string>, Dictionary<string, double>>((_, p, s) =>
                {
                    properties = p;
                    stats = s;
                });

            await service.BuildMetrics(2, 1920, 1, CancellationToken.None);

            properties.Keys.Should().Contain("JobId");
            properties.Keys.Should().Contain("CollectionPeriod");
            properties.Keys.Should().Contain("AcademicYear");

            stats.Keys.Should().Contain("Percentage");
            stats.Keys.Should().Contain("ContractType1");
            stats.Keys.Should().Contain("ContractType2");
            stats.Keys.Should().Contain("DifferenceContractType1");
            stats.Keys.Should().Contain("DifferenceContractType2");
            stats.Keys.Should().Contain("PercentageContractType1");
            stats.Keys.Should().Contain("PercentageContractType2");
            stats.Keys.Should().Contain("EarningsDCContractType1");
            stats.Keys.Should().Contain("EarningsDCContractType2");
            stats.Keys.Should().Contain("PaymentsContractType1");
            stats.Keys.Should().Contain("PaymentsContractType2");
            stats.Keys.Should().Contain("DataLockedEarnings");
            stats.Keys.Should().Contain("DataLockedEarnings16To18");
            stats.Keys.Should().Contain("DataLockedEarnings19Plus");
            stats.Keys.Should().Contain("AlreadyPaidDataLockedEarnings");
            stats.Keys.Should().Contain("AlreadyPaidDataLockedEarnings16To18");
            stats.Keys.Should().Contain("AlreadyPaidDataLockedEarnings19Plus");
            stats.Keys.Should().Contain("TotalDataLockedEarnings");
            stats.Keys.Should().Contain("TotalDataLockedEarnings16To18");
            stats.Keys.Should().Contain("TotalDataLockedEarnings19Plus");
            stats.Keys.Should().Contain("HeldBackCompletionPaymentsContractType1");
            stats.Keys.Should().Contain("HeldBackCompletionPaymentsContractType2");
            stats.Keys.Should().Contain("PaymentsYearToDateContractType1");
            stats.Keys.Should().Contain("PaymentsYearToDateContractType2");

            stats.Keys.Should().Contain("PercentageContractType1");
            stats.Keys.Should().Contain("PercentageContractType2");
            stats.Keys.Should().Contain("Total");
            stats.Keys.Should().Contain("DifferenceTotal");
            stats.Keys.Should().Contain("EarningsDCTotal");
            stats.Keys.Should().Contain("PaymentsTotal");
            stats.Keys.Should().Contain("HeldBackCompletionPaymentsTotal");
            stats.Keys.Should().Contain("PaymentsYearToDateTotal");

            stats.Keys.Should().Contain("PaymentsYearToDateContractType2");
            stats.Keys.Should().Contain("PaymentsYearToDateContractType2");
            stats.Keys.Should().Contain("PaymentsYearToDateContractType2");

            stats.Keys.Should().Contain("ContractType1FundingSourceTotal");
            stats.Keys.Should().Contain("ContractType1FundingSource1");
            stats.Keys.Should().Contain("ContractType1FundingSource2");
            stats.Keys.Should().Contain("ContractType1FundingSource3");
            stats.Keys.Should().Contain("ContractType1FundingSource4");
            stats.Keys.Should().Contain("ContractType1FundingSource5");

            stats.Keys.Should().Contain("ContractType2FundingSourceTotal");
            stats.Keys.Should().Contain("ContractType2FundingSource1");
            stats.Keys.Should().Contain("ContractType2FundingSource2");
            stats.Keys.Should().Contain("ContractType2FundingSource3");
            stats.Keys.Should().Contain("ContractType2FundingSource4");
            stats.Keys.Should().Contain("ContractType2FundingSource5");

            stats.Keys.Should().Contain("ContractType1TransactionTypeTotal");
            stats.Keys.Should().Contain("ContractType1TransactionType01");
            stats.Keys.Should().Contain("ContractType1TransactionType02");
            stats.Keys.Should().Contain("ContractType1TransactionType03");
            stats.Keys.Should().Contain("ContractType1TransactionType04");
            stats.Keys.Should().Contain("ContractType1TransactionType05");
            stats.Keys.Should().Contain("ContractType1TransactionType06");
            stats.Keys.Should().Contain("ContractType1TransactionType07");
            stats.Keys.Should().Contain("ContractType1TransactionType08");
            stats.Keys.Should().Contain("ContractType1TransactionType09");
            stats.Keys.Should().Contain("ContractType1TransactionType10");
            stats.Keys.Should().Contain("ContractType1TransactionType11");
            stats.Keys.Should().Contain("ContractType1TransactionType12");
            stats.Keys.Should().Contain("ContractType1TransactionType13");
            stats.Keys.Should().Contain("ContractType1TransactionType14");
            stats.Keys.Should().Contain("ContractType1TransactionType15");
            stats.Keys.Should().Contain("ContractType1TransactionType16");

            stats.Keys.Should().Contain("ContractType2TransactionTypeTotal");
            stats.Keys.Should().Contain("ContractType2TransactionType01");
            stats.Keys.Should().Contain("ContractType2TransactionType02");
            stats.Keys.Should().Contain("ContractType2TransactionType03");
            stats.Keys.Should().Contain("ContractType2TransactionType04");
            stats.Keys.Should().Contain("ContractType2TransactionType05");
            stats.Keys.Should().Contain("ContractType2TransactionType06");
            stats.Keys.Should().Contain("ContractType2TransactionType07");
            stats.Keys.Should().Contain("ContractType2TransactionType08");
            stats.Keys.Should().Contain("ContractType2TransactionType09");
            stats.Keys.Should().Contain("ContractType2TransactionType10");
            stats.Keys.Should().Contain("ContractType2TransactionType11");
            stats.Keys.Should().Contain("ContractType2TransactionType12");
            stats.Keys.Should().Contain("ContractType2TransactionType13");
            stats.Keys.Should().Contain("ContractType2TransactionType14");
            stats.Keys.Should().Contain("ContractType2TransactionType15");
            stats.Keys.Should().Contain("ContractType2TransactionType16");

            stats.Keys.Should().Contain("DataLockedCountDLock1");
            stats.Keys.Should().Contain("DataLockedCountDLock2");
            stats.Keys.Should().Contain("DataLockedCountDLock3");
            stats.Keys.Should().Contain("DataLockedCountDLock4");
            stats.Keys.Should().Contain("DataLockedCountDLock5");
            stats.Keys.Should().Contain("DataLockedCountDLock6");
            stats.Keys.Should().Contain("DataLockedCountDLock7");
            stats.Keys.Should().Contain("DataLockedCountDLock8");
            stats.Keys.Should().Contain("DataLockedCountDLock9");
            stats.Keys.Should().Contain("DataLockedCountDLock10");
            stats.Keys.Should().Contain("DataLockedCountDLock11");
            stats.Keys.Should().Contain("DataLockedCountDLock12");

            stats.Keys.Should().Contain("InLearning");
        }

        public class PeriodEndProviderSummaryFake : IPeriodEndProviderSummary
        {
            public bool AddDcEarningsCalled { get; private set; }
            public bool AddTransactionTypesCalled { get; private set; }
            public bool AddFundingSourceAmountsCalled { get; private set; }
            public bool AddDataLockedEarningsCalled { get; private set; }
            public bool AddPeriodEndProviderDataLockTypeCountsCalled { get; private set; }
            public bool AddDataLockedAlreadyPaidCalled { get; private set; }
            public bool AddPaymentsYearToDateCalled { get; private set; }
            public bool AddHeldBackCompletionPaymentsCalled { get; private set; }
            public bool AddInLearningCountCalled { get; private set; }
            public bool AddNegativeEarningsCalled { get; private set; }

            public ProviderPeriodEndSummaryModel GetMetrics()
            {
                return new ProviderPeriodEndSummaryModel();
            }

            public void AddDcEarnings(IEnumerable<TransactionTypeAmountsByContractType> source) { AddDcEarningsCalled = true; }
            public void AddTransactionTypes(IEnumerable<TransactionTypeAmountsByContractType> transactionTypes) { AddTransactionTypesCalled = true; }
            public void AddFundingSourceAmounts(IEnumerable<ProviderFundingSourceAmounts> fundingSourceAmounts) { AddFundingSourceAmountsCalled = true; }

            public void AddDataLockedEarnings(ProviderFundingLineTypeAmounts dataLockedEarningsTotal) { AddDataLockedEarningsCalled = true; }
            public void AddPeriodEndProviderDataLockTypeCounts(PeriodEndProviderDataLockTypeCounts periodEndProviderDataLockTypeCounts)
            {
                AddPeriodEndProviderDataLockTypeCountsCalled = true;
            }

            public void AddDataLockedAlreadyPaid(ProviderFundingLineTypeAmounts dataLockedAlreadyPaidTotal) { AddDataLockedAlreadyPaidCalled = true; }

            public void AddPaymentsYearToDate(ProviderContractTypeAmounts paymentsYearToDate) { AddPaymentsYearToDateCalled = true; }

            public void AddHeldBackCompletionPayments(ProviderContractTypeAmounts heldBackCompletionPayments) { AddHeldBackCompletionPaymentsCalled = true; }
            public void AddInLearningCount(ProviderInLearningTotal inLearningTotal) { AddInLearningCountCalled = true; }
            public void AddNegativeEarnings(NegativeEarningsContractTypeAmounts providerNegativeEarnings) { AddNegativeEarningsCalled = true; }
        }
    }
}