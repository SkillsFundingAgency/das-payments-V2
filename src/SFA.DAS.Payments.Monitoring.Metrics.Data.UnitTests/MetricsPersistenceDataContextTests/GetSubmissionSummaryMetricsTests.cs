using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.UnitTests.MetricsPersistenceDataContextTests
{
    [TestFixture]
    public class GetSubmissionSummaryMetricsTests
    {
        private SubmissionMetricsRepositoryFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new SubmissionMetricsRepositoryFixture();
        }

        [Test]
        public async Task AndMatchingSummariesExist_ThenMatchingSummariesReturned()
        {
            fixture.With_SubmissionSummaries_InDb();

            var result = await fixture.GetSubmissionsSummaryMetrics();

            fixture.Assert_MatchingSubmissionSummaries_AreReturned(result);
        }

        [Test]
        public async Task AndNoMatchingSummariesExist_ThenNullReturned()
        {
            var result = await fixture.GetSubmissionsSummaryMetrics();

            result.Should().BeNull();
        }
    }

    internal class SubmissionMetricsRepositoryFixture
    {
        private readonly long jobId;
        private readonly short academicYear;
        private readonly byte currentCollectionPeriod;
        private readonly MetricsPersistenceDataContext persistenceDataContext;
        private readonly Mock<IMetricsQueryDataContextFactory> metricsQueryDataContextFactory;
        private readonly Mock<IPaymentLogger> logger;
        private readonly SubmissionMetricsRepository sut;

        private readonly List<SubmissionSummaryModel> matchingSubmissionSummaries;
        private readonly List<SubmissionSummaryModel> nonMatchingSubmissionSummaries;

        public SubmissionMetricsRepositoryFixture()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            jobId = fixture.Create<long>();
            academicYear = fixture.Create<short>();
            currentCollectionPeriod = fixture.Create<byte>();
            persistenceDataContext = new InMemoryMetricsPersistenceDataContext();
            metricsQueryDataContextFactory = new Mock<IMetricsQueryDataContextFactory>();
            logger = new Mock<IPaymentLogger>();
            sut = new SubmissionMetricsRepository(persistenceDataContext, metricsQueryDataContextFactory.Object, logger.Object);

            matchingSubmissionSummaries = fixture.Create<List<SubmissionSummaryModel>>();
            nonMatchingSubmissionSummaries = fixture.Create<List<SubmissionSummaryModel>>();

            matchingSubmissionSummaries.ForEach(x =>
            {
                x.AcademicYear = academicYear;
                x.CollectionPeriod = currentCollectionPeriod;
            });
        }

        public Task<IList<SubmissionSummaryModel>> GetSubmissionsSummaryMetrics() => sut.GetSubmissionsSummaryMetrics(jobId, academicYear, currentCollectionPeriod, It.IsAny<CancellationToken>());

        public SubmissionMetricsRepositoryFixture With_SubmissionSummaries_InDb()
        {
            persistenceDataContext.SubmissionSummaries.AddRange(matchingSubmissionSummaries);
            persistenceDataContext.SubmissionSummaries.AddRange(nonMatchingSubmissionSummaries);
            persistenceDataContext.SaveChangesAsync();

            return this;
        }

        public void Assert_MatchingSubmissionSummaries_AreReturned(IList<SubmissionSummaryModel> result)
        {
            result.Count.Should().Be(matchingSubmissionSummaries.Count);
            matchingSubmissionSummaries.ForEach(x => result.Should().Contain(x));
            nonMatchingSubmissionSummaries.ForEach(x => result.Should().NotContain(x));
        }
    }
}