using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionJobsRepositoryTests
    {
        [Test, AutoMoqData]
        public async Task WhenThereAreSuccessfulJobs_ShouldReturn_SuccessfulJobs(
            [Frozen] ISubmissionJobsDataContext context,
            SubmissionJobsRepository sut,
            short academicYear,
            byte collectionPeriod,
            long ukprn
        )
        {
            context.LatestSuccessfulJobs.Add(new LatestSuccessfulJobModel
            {
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                Ukprn = ukprn,
            });
            await (context as InMemorySubmissionJobsDataContext).SaveChangesAsync();

            var actual = await sut.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod);
            actual.Should().HaveCount(1);
        }

        [Test, AutoMoqData]
        public async Task WhenThereAreNoSuccessfulJobsForPeriod_ShouldReturn_EmptyList(
            [Frozen] ISubmissionJobsDataContext context,
            SubmissionJobsRepository sut,
            short academicYear,
            byte collectionPeriod,
            long ukprn
        )
        {
            context.LatestSuccessfulJobs.Add(new LatestSuccessfulJobModel
            {
                AcademicYear = (short)(academicYear + 1),
                CollectionPeriod = collectionPeriod,
                Ukprn = ukprn,
            });
            await (context as InMemorySubmissionJobsDataContext).SaveChangesAsync();

            var actual = await sut.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod);
            actual.Should().BeEmpty();
        }

        [Test, AutoMoqData]
        public async Task WhenThereAreNoSuccessfulJobs_ShouldReturn_EmptyList(
            [Frozen] ISubmissionJobsDataContext context,
            SubmissionJobsRepository sut,
            short academicYear,
            byte collectionPeriod,
            long ukprn
        )
        {
            var actual = await sut.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod);
            actual.Should().BeEmpty();
        }
    }
}
