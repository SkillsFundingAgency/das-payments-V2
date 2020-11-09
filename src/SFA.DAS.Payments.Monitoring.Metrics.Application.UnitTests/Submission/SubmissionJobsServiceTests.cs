using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionJobsServiceTests
    {
        [Test, AutoMoqData]
        public async Task DcJobId_IsCorrectlyMapped_ToJobId(
            [Frozen] Mock<ISubmissionJobsRepository> repository,
            SubmissionJobsService sut,
            List<LatestSuccessfulJobModel> testJobs,
            short academicYear,
            byte collectionPeriod
        )
        {
            repository.Setup(x => x.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod))
                .ReturnsAsync(testJobs);

            var actual = await sut.SuccessfulSubmissionsForCollectionPeriod(academicYear, collectionPeriod);

            foreach (var job in testJobs)
            {
                actual.SuccessfulSubmissionJobs.Should().ContainEquivalentOf(new {JobId = job.DcJobId});
            }
        }

        [Test, AutoMoqData]
        public async Task CorrectCombinationOfJobIdAndUkprn_IsCorrectlyMapped(
            [Frozen] Mock<ISubmissionJobsRepository> repository,
            SubmissionJobsService sut,
            List<LatestSuccessfulJobModel> testJobs,
            short academicYear,
            byte collectionPeriod
        )
        {
            repository.Setup(x => x.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod))
                .ReturnsAsync(testJobs);

            var actual = await sut.SuccessfulSubmissionsForCollectionPeriod(academicYear, collectionPeriod);

            foreach (var job in testJobs)
            {
                actual.SuccessfulSubmissionJobs.Should().ContainEquivalentOf(new {job.Ukprn, JobId = job.DcJobId});
            }
        }

        [Test, AutoMoqData]
        public async Task CorrectNumberOfResultsAreMapped(
            [Frozen] Mock<ISubmissionJobsRepository> repository,
            SubmissionJobsService sut,
            List<LatestSuccessfulJobModel> testJobs,
            short academicYear,
            byte collectionPeriod
        )
        {
            repository.Setup(x => x.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod))
                .ReturnsAsync(testJobs);

            var actual = await sut.SuccessfulSubmissionsForCollectionPeriod(academicYear, collectionPeriod);

            actual.SuccessfulSubmissionJobs.Should().HaveCount(testJobs.Count);
        }
    }
}
