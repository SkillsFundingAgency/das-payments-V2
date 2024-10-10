using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Autofac.Extras.Moq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.WindowsAzure.Storage.File.Protocol;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionMetricsServiceDataTests
    {
        private AutoMock moqer;
        private List<TransactionTypeAmounts> dcEarnings;
        private List<TransactionTypeAmounts> dasEarnings;
        private LatestSuccessfulJobModel latestSuccessfulJob;
        private DataLockTypeCounts dataLocks;
        private InMemoryMetricsQueryDataContext inMemoryMetricsQueryDataContext;
        private Mock<ISubmissionMetricsRepository> submissionMetricsRepository;

        [SetUp]
        public void SetUp()
        {
            inMemoryMetricsQueryDataContext = new InMemoryMetricsQueryDataContext();

            moqer = AutoMock.GetLoose();
            dcEarnings = TestsHelper.DefaultDcEarnings;
            dasEarnings = TestsHelper.DefaultDasEarnings;
            dataLocks = TestsHelper.DefaultDataLockedEarnings;

            var dcMetricsDataContext = moqer.Mock<IDcMetricsDataContext>();
            dcMetricsDataContext.Setup(ctx => ctx.GetEarnings(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dcEarnings);

            moqer.Mock<IDcMetricsDataContextFactory>()
                .Setup(factory => factory.CreateContext(It.IsAny<short>()))
                .Returns(dcMetricsDataContext.Object);

            latestSuccessfulJob = moqer.Mock<LatestSuccessfulJobModel>().Object;
            latestSuccessfulJob.Ukprn = 1234;
            latestSuccessfulJob.CollectionPeriod = 1;
            latestSuccessfulJob.AcademicYear = 1920;
            latestSuccessfulJob.DcJobId = 123;

            moqer.Mock<ISubmissionSummaryFactory>()
                .Setup(factory =>
                    factory.Create(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .Returns((long ukprn, long jobId, short academicYear, byte collectionPeriod) => new SubmissionSummary(ukprn, jobId, collectionPeriod, academicYear));

            var jobsRepository = moqer.Mock<ISubmissionJobsRepository>();

            jobsRepository.Setup(x =>
                    x.GetLatestSuccessfulJobForProvider(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(latestSuccessfulJob);

            moqer.Mock<ISubmissionJobsDataContext>();

            submissionMetricsRepository = moqer.Mock<ISubmissionMetricsRepository>();

            submissionMetricsRepository.Setup(repo => repo.GetDasEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dasEarnings);
            submissionMetricsRepository.Setup(repo => repo.GetDasEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dasEarnings);
            submissionMetricsRepository.Setup(repo => repo.GetDataLockedEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dataLocks);
            submissionMetricsRepository.Setup(repo => repo.GetDataLockedEarningsTotal(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.DefaultDataLockedTotal);
            submissionMetricsRepository.Setup(repo => repo.GetAlreadyPaidDataLockedEarnings(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.AlreadyPaidDataLockedEarnings);
            submissionMetricsRepository.Setup(repo => repo.GetHeldBackCompletionPaymentsTotal(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.DefaultHeldBackCompletionPayments);
            submissionMetricsRepository.Setup(repo => repo.GetYearToDatePaymentsTotal(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestsHelper.DefaultYearToDateAmounts);


            var queryDataContext = moqer.Mock<IMetricsQueryDataContextFactory>();

            queryDataContext.Setup(f => f.Create())
                .Returns(inMemoryMetricsQueryDataContext);

            var realSubmissionMetricsRepository = moqer.Create<SubmissionMetricsRepository>(
                new NamedParameter("metricsQueryDataContextFactory", queryDataContext.Object),
                new AutowiringParameter());

            submissionMetricsRepository
                .Setup(repo => repo.GetRequiredPayments(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .Returns(async (long ukprn, long jobId, CancellationToken cancellationToken) => await realSubmissionMetricsRepository.GetRequiredPayments(ukprn, jobId, cancellationToken));
        }

        [Test]
        public async Task Includes_ClawbackPayments_In_Required_Payments()
        {
            await inMemoryMetricsQueryDataContext.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
            {
                Id = 1,
                EventId = Guid.NewGuid(),
                Ukprn = 1234,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1 },
                Amount = 11000,
                TransactionType = TransactionType.Learning,
                ContractType = ContractType.Act1
            });

            await inMemoryMetricsQueryDataContext.RequiredPaymentEvents.AddAsync(new RequiredPaymentEventModel
            {
                Id = 2,
                EventId = Guid.NewGuid(),
                Ukprn = 1234,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1 },
                Amount = 15000,
                TransactionType = TransactionType.Learning,
                ContractType = ContractType.Act2
            });

            await inMemoryMetricsQueryDataContext.Payments.AddAsync(new PaymentModel
            {
                Id = 3,
                EventId = Guid.NewGuid(),
                Ukprn = 1234,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1 },
                Amount = 300,
                FundingSource = FundingSourceType.CoInvestedSfa,
                TransactionType = TransactionType.Learning,
                ContractType = ContractType.Act1,
                ClawbackSourcePaymentEventId = Guid.NewGuid(),
                FundingPlatformType = FundingPlatformType.SubmitLearnerData
            });

            await inMemoryMetricsQueryDataContext.Payments.AddAsync(new PaymentModel
            {
                Id = 4,
                EventId = Guid.NewGuid(),
                Ukprn = 1234,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1 },
                Amount = 300,
                FundingSource = FundingSourceType.FullyFundedSfa,
                TransactionType = TransactionType.Learning,
                ContractType = ContractType.Act2,
                ClawbackSourcePaymentEventId = Guid.NewGuid(),
                FundingPlatformType = FundingPlatformType.SubmitLearnerData
            });

            await inMemoryMetricsQueryDataContext.Payments.AddAsync(new PaymentModel
            {
                Id = 5,
                EventId = Guid.NewGuid(),
                Ukprn = 1234,
                JobId = 123,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 1 },
                Amount = 300,
                FundingSource = FundingSourceType.Levy,
                TransactionType = TransactionType.Learning,
                ContractType = ContractType.Act1,
                ClawbackSourcePaymentEventId = Guid.NewGuid(),
                FundingPlatformType = FundingPlatformType.SubmitLearnerData
            });

            await inMemoryMetricsQueryDataContext.SaveChangesAsync();

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            submissionMetricsRepository
                .Verify(x => x.SaveSubmissionMetrics(It.Is<SubmissionSummaryModel>(s => s.RequiredPayments.ContractType1 == 11300 && s.RequiredPayments.ContractType2 == 15300), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task WhenLatestSuccessfulJobExists_AndDoesNotMatchMessageJobId_ThenDoesNotBuildMetrics()
        {

            latestSuccessfulJob.DcJobId = 999;

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummaryFactory>().Verify(x => x.Create(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()),Times.Never);
        }

        [Test]
        public async Task WhenLatestSuccessfulJobExists_AndMessageJobIdMatches_ThenBuildsMetrics()
        {

            latestSuccessfulJob.DcJobId = 123;

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummaryFactory>().Verify(x => x.Create(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()),Times.Once);
        }

        [Test]
        public async Task WhenLatestSuccessfulJobIsNull_ThenBuildsMetrics()
        {
            LatestSuccessfulJobModel job = null;
            var jobsRepository = moqer.Mock<ISubmissionJobsRepository>();

            jobsRepository.Setup(x =>
                    x.GetLatestSuccessfulJobForProvider(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(job);

            var service = moqer.Create<SubmissionMetricsService>();
            await service.BuildMetrics(1234, 123, 1920, 1, CancellationToken.None).ConfigureAwait(false);

            moqer.Mock<ISubmissionSummaryFactory>().Verify(x => x.Create(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()),Times.Once);
        }
    }
}