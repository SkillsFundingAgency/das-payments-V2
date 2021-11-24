using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests.JobsDataContextTests
{
    [TestFixture]
    public class DoSubmissionSummariesExistForJobsTests
    {
        [Test]
        public async Task When_SubmissionSummary_Exists_For_Given_JobId_Then_The_Result_Does_Not_Include_That_JobId()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.SubmissionSummaries.AddAsync(new SubmissionSummaryModel
                {
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    JobId = 1,
                    Ukprn = 1,
                });

                await sut.SaveChangesAsync();

                var outstandingJobs = new List<OutstandingJobResult> { new OutstandingJobResult { DcJobId = 1 } };

                var actual = sut.DoSubmissionSummariesExistForJobs(outstandingJobs);

                actual.Count.Should().Be(0);
            }
        }

        [Test]
        public async Task When_One_SubmissionSummary_Exists_For_Given_List_Of_JobIds_Then_The_Result_Does_Only_Include_Outstanding_JobId()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                await sut.SubmissionSummaries.AddAsync(new SubmissionSummaryModel
                {
                    AcademicYear = 2122,
                    CollectionPeriod = 1,
                    JobId = 1,
                    Ukprn = 1,
                });

                await sut.SaveChangesAsync();

                var outstandingJobs = new List<OutstandingJobResult> { new OutstandingJobResult { DcJobId = 1, Ukprn = 1 }, new OutstandingJobResult { DcJobId = 2, Ukprn = 2 } };

                var actual = sut.DoSubmissionSummariesExistForJobs(outstandingJobs);

                actual.Count.Should().Be(1);
                actual[0].Should().Be(2);
            }
        }

        [Test]
        public void When_All_SubmissionSummaries_AreOutStanding_For_Given_List_Of_JobIds_Then_The_Result_Includes_All_Outstanding_JobId()
        {
            using (var sut = new InMemoryMetricsQueryDataContext())
            {
                //Do nothing in DB setup as we are asserting that all the jobs are outstanding

                var outstandingJobs = new List<OutstandingJobResult> { new OutstandingJobResult { DcJobId = 1, Ukprn = 1 }, new OutstandingJobResult { DcJobId = 2, Ukprn = 2 } };

                var actual = sut.DoSubmissionSummariesExistForJobs(outstandingJobs);

                actual.Count.Should().Be(2);
                actual[0].Should().Be(1);
                actual[1].Should().Be(2);
            }
        }
    }
}
