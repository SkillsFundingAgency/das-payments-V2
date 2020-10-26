using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data.UnitTests.MetricsPersistenceDataContextTests
{
    [TestFixture]
    public class GetCollectionPeriodToleranceTests
    {
        private GetCollectionPeriodToleranceFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new GetCollectionPeriodToleranceFixture();
        }

        [Test]
        public async Task AndMatchingCollectionPeriodToleranceModelExists_ThenModelReturned()
        {
            fixture.With_CollectionPeriodToleranceModelsInDb();

            var result = await fixture.Act();

            fixture.Assert_CorrectMatchingModelIsReturned(result);
        }

        [Test]
        public async Task AndNonMatchingCollectionPeriodToleranceModelsExist_ThenReturnsNull()
        {
            fixture.With_NonMatchingCollectionPeriodToleranceModelsInDb();

            var result = await fixture.Act();

            result.Should().BeNull();
        }

        [Test]
        public async Task AndNothingInDb_ThenReturnsNull()
        {
            var result = await fixture.Act();

            result.Should().BeNull();
        }
    }

    internal class GetCollectionPeriodToleranceFixture
    {
        private readonly short academicYear;
        private readonly byte collectionPeriod;
        private readonly MetricsPersistenceDataContext persistenceDataContext;
        private readonly Mock<IMetricsQueryDataContextFactory> metricsQueryDataContextFactory;
        private readonly Mock<IPaymentLogger> logger;
        private readonly SubmissionMetricsRepository sut;

        private readonly CollectionPeriodToleranceModel matchingCollectionPeriodToleranceModel;
        private readonly List<CollectionPeriodToleranceModel> nonMatchingCollectionPeriodToleranceModels;

        public GetCollectionPeriodToleranceFixture()
        {
            var fixture = new Fixture();

            academicYear = fixture.Create<short>();
            collectionPeriod = fixture.Create<byte>();
            persistenceDataContext = new InMemoryMetricsPersistenceDataContext();
            metricsQueryDataContextFactory = new Mock<IMetricsQueryDataContextFactory>();
            logger = new Mock<IPaymentLogger>();
            sut = new SubmissionMetricsRepository(persistenceDataContext, metricsQueryDataContextFactory.Object, logger.Object);

            matchingCollectionPeriodToleranceModel = fixture.Create<CollectionPeriodToleranceModel>();
            matchingCollectionPeriodToleranceModel.AcademicYear = academicYear;
            matchingCollectionPeriodToleranceModel.CollectionPeriod = collectionPeriod;
            nonMatchingCollectionPeriodToleranceModels = fixture.Create<List<CollectionPeriodToleranceModel>>();
        }

        public Task<CollectionPeriodToleranceModel> Act() => sut.GetCollectionPeriodTolerance(collectionPeriod, academicYear, It.IsAny<CancellationToken>());

        public GetCollectionPeriodToleranceFixture With_CollectionPeriodToleranceModelsInDb()
        {
            persistenceDataContext.CollectionPeriodTolerances.Add(matchingCollectionPeriodToleranceModel);
            persistenceDataContext.CollectionPeriodTolerances.AddRange(nonMatchingCollectionPeriodToleranceModels);
            persistenceDataContext.SaveChanges();
            return this;
        }

        public GetCollectionPeriodToleranceFixture With_NonMatchingCollectionPeriodToleranceModelsInDb()
        {
            persistenceDataContext.CollectionPeriodTolerances.AddRange(nonMatchingCollectionPeriodToleranceModels);
            persistenceDataContext.SaveChanges();
            return this;
        }

        public void Assert_CorrectMatchingModelIsReturned(CollectionPeriodToleranceModel result)
        {
            matchingCollectionPeriodToleranceModel.Should().BeSameAs(result);
            matchingCollectionPeriodToleranceModel.AcademicYear.Should().Be(result.AcademicYear);
            matchingCollectionPeriodToleranceModel.CollectionPeriod.Should().Be(result.CollectionPeriod);
        }
    }
}