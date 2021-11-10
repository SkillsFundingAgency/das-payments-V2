using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobsDataContextTests
{
    [TestFixture]
    public class GetOutstandingOrTimedOutJobsTests
    {
        [Test]
        public async Task IgnoresThePeriodEndStartJobAndReturnsEmptyResult()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.Jobs.AddAsync(new JobModel
                {
                    JobType = JobType.PeriodEndStartJob,
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    DcJobId = 1,
                    Ukprn = 1,
                    Status = JobStatus.InProgress,
                    DcJobSucceeded = true,
                    StartTime = DateTimeOffset.Now
                });

                await sut.SaveChangesAsync();

                var periodEndStartJob = new JobModel { JobType = JobType.PeriodEndStartJob, AcademicYear = 2122, CollectionPeriod = 1, StartTime = DateTimeOffset.Now, DcJobId = 123 };

                var actual = await sut.GetOutstandingOrTimedOutJobs(periodEndStartJob, CancellationToken.None);

                actual.Count.Should().Be(0);
            }
        }
        
        [Test]
        public async Task WhenThereAreTimedOutJobsOlderThen150Min_thenThoseJobsAreIgnoredAndReturnsEmptyResult()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.Jobs.AddAsync(new JobModel
                {
                    JobType = JobType.EarningsJob,
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    DcJobId = 1,
                    Ukprn = 1,
                    Status = JobStatus.TimedOut,
                    DcJobSucceeded = true,
                    StartTime = DateTimeOffset.Now.AddMinutes(-151)
                });

                await sut.SaveChangesAsync();

                var periodEndStartJob = new JobModel { JobType = JobType.PeriodEndStartJob, AcademicYear = 2122, CollectionPeriod = 1, StartTime = DateTimeOffset.Now, DcJobId = 123 };

                var actual = await sut.GetOutstandingOrTimedOutJobs(periodEndStartJob, CancellationToken.None);

                actual.Count.Should().Be(0);
            }
        }
        
        [Test]
        public async Task WhenThereAreTimedOutJobs_thenThoseJobsAreIncludedInResult()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.Jobs.AddAsync(new JobModel
                {
                    JobType = JobType.EarningsJob,
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    DcJobId = 1,
                    Ukprn = 1,
                    Status = JobStatus.TimedOut,
                    DcJobSucceeded = true,
                    StartTime = DateTimeOffset.Now.AddMinutes(-149)
                });

                await sut.SaveChangesAsync();

                var periodEndStartJob = new JobModel { JobType = JobType.PeriodEndStartJob, AcademicYear = 2122, CollectionPeriod = 1, StartTime = DateTimeOffset.Now, DcJobId = 123 };

                var actual = await sut.GetOutstandingOrTimedOutJobs(periodEndStartJob, CancellationToken.None);

                actual.Count.Should().Be(1);
                actual[0].DcJobId.Should().Be(1);
                actual[0].Ukprn.Should().Be(1);
            }
        }
        
        [Test]
        public async Task WhenThereAreOutstandingJobsOlderThen150Min_thenThoseJobsAreIgnoredAndReturnsEmptyResult()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.Jobs.AddAsync(new JobModel
                {
                    JobType = JobType.EarningsJob,
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    DcJobId = 1,
                    Ukprn = 1,
                    Status = JobStatus.InProgress,
                    DcJobSucceeded = true,
                    StartTime = DateTimeOffset.Now.AddMinutes(-151)
                });

                await sut.SaveChangesAsync();

                var periodEndStartJob = new JobModel { JobType = JobType.PeriodEndStartJob, AcademicYear = 2122, CollectionPeriod = 1, StartTime = DateTimeOffset.Now, DcJobId = 123 };

                var actual = await sut.GetOutstandingOrTimedOutJobs(periodEndStartJob, CancellationToken.None);

                actual.Count.Should().Be(0);
            }
        }

        [Test]
        public async Task WhenThereAreJobsFromPreviousAcademicYearAndCurrentAcademicYear_thenOnlyReturnsJobsFromCurrentAcademicYear()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.Jobs.AddAsync(new JobModel
                {
                    JobType = JobType.EarningsJob,
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    DcJobId = 1,
                    Ukprn = 1,
                    Status = JobStatus.InProgress,
                    DcJobSucceeded = true,
                    StartTime = DateTimeOffset.Now.AddMinutes(-148)
                });

                await sut.Jobs.AddAsync(new JobModel
                {
                    JobType = JobType.EarningsJob,
                    AcademicYear = 2021,
                    CollectionPeriod = 14,
                    DcJobId = 14,
                    Ukprn = 14,
                    Status = JobStatus.InProgress,
                    DcJobSucceeded = true,
                    StartTime = DateTimeOffset.Now.AddMinutes(-148)
                });

                await sut.SaveChangesAsync();

                var periodEndStartJob = new JobModel { JobType = JobType.PeriodEndStartJob, AcademicYear = 2122, CollectionPeriod = 1, StartTime = DateTimeOffset.Now, DcJobId = 123 };

                var actual = await sut.GetOutstandingOrTimedOutJobs(periodEndStartJob, CancellationToken.None);

                actual.Count.Should().Be(1);
                actual[0].DcJobId.Should().Be(1);
                actual[0].Ukprn.Should().Be(1);
            }
        }

        [Test]
        public async Task WhenThereAreNoOutstandingJobs_thenReturnsEmptyResult()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                var periodEndStartJob = new JobModel { JobType = JobType.PeriodEndStartJob, AcademicYear = 2122, CollectionPeriod = 1, StartTime = DateTimeOffset.Now, DcJobId = 123 };

                var actual = await sut.GetOutstandingOrTimedOutJobs(periodEndStartJob, CancellationToken.None);

                actual.Count.Should().Be(0);
            }
        }
    }
}
