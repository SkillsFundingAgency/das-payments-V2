using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data;
using SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Data.Entities;
using SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Infrastructure;
using TechTalk.SpecFlow;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace SFA.DAS.Payments.ScheduledJobs.AcceptanceTests.Steps
{
    [Binding]
    public class AuditDataCleanUpSteps
    {
        private readonly List<SubmissionData> submissions;
        private readonly SubmissionDataContext submissionDataContext;
        private readonly long ukprn;
        private readonly Random randomNumberGenerator;

        public AuditDataCleanUpSteps()
        {
            var config = new TestConfiguration();
            submissionDataContext = new SubmissionDataContext(config.PaymentsConnectionString);
            randomNumberGenerator = new Random(Guid.NewGuid().GetHashCode());
            ukprn = randomNumberGenerator.Next(1_000_000);
            submissions = new List<SubmissionData>();
        }

        [Given(@"a Provider has done two submissions, First Submission (.*) and Second Submission (.*) in collectionPeriod (.*)")]
        public async Task GivenAProviderHasDoneTwoSubmissions(string submissionId1, string submissionId2, byte collectionPeriod)
        {
            submissions.Add(await CreateSubmission(submissionId1, collectionPeriod, 1920));
            submissions.Add(await CreateSubmission(submissionId2, collectionPeriod, 1920));
        }

        [Given(@"Now does two new submissions, First Submission (.*) and Second Submission (.*) in collection period (.*)")]
        public async Task GivenAProviderHasDoneTwoSubmissionsInCollectionPeriod(string submissionId1, string submissionId2, byte collectionPeriod)
        {
            submissions.Add(await CreateSubmission(submissionId1, collectionPeriod, 1920));
            submissions.Add(await CreateSubmission(submissionId2, collectionPeriod, 1920));
        }

        [Given(@"a Provider has done one submissions, Submission (.*) in collection period (.*)")]
        public async Task GivenAProviderHasDoneTwoSubmissionsFirstSubmissionAAndSecondSubmissionB(string submissionId, byte collectionPeriod)
        {
            submissions.Add(await CreateSubmission(submissionId, collectionPeriod, 1920));
        }

        [Given(@"Submission (.*) has status (.*) from collection period (.*)")]
        public async Task GivenSubmissionStatusInCollectionPeriod(string submissionId, string status, byte collectionPeriod)
        {
            await SetJobStatus(submissions.SingleOrDefault(s => s.SubmissionId == submissionId && s.CollectionPeriod == collectionPeriod), status);
        }

        [Given(@"Both Submission has status (.*) in collection period (.*)")]
        public async Task GivenBothSubmissionStatusInCollectionPeriod(string status, byte collectionPeriod)
        {
            foreach (var submission in submissions.Where(s => collectionPeriod == 0 || s.CollectionPeriod == collectionPeriod))
            {
                await SetJobStatus(submission, status);
            }
        }

        [Given(@"Submission (.*) has DCJobSucceeded (.*) from collection period (.*)")]
        public async Task GivenSubmissionAHasDcJobSucceededNullFromCollectionPeriod(string submissionId, string dcJobSucceeded, byte collectionPeriod)
        {
            await SetDcJobSucceeded(submissions.SingleOrDefault(s => s.SubmissionId == submissionId && s.CollectionPeriod == collectionPeriod), dcJobSucceeded);
        }

        [When(@"Audit Data Cleanup Function is executed in collectionPeriod 2")]
        public async Task WhenAuditDataCleanupFunctionIsExecuted()
        {
            var httpClient = new HttpClient();

            var earningEventresult = await httpClient.GetAsync(TestConfiguration.EarningFunctionUrl);
            var fundingSourceEventresult = await httpClient.GetAsync(TestConfiguration.FundingSourceFunctionUrl);
            var requiredPaymentEventresult = await httpClient.GetAsync(TestConfiguration.RequiredPaymentFunctionUrl);
            var dataLocEventresult = await httpClient.GetAsync(TestConfiguration.DataLockEvenFunctionUrl);

            earningEventresult.EnsureSuccessStatusCode();
            fundingSourceEventresult.EnsureSuccessStatusCode();
            requiredPaymentEventresult.EnsureSuccessStatusCode();
            dataLocEventresult.EnsureSuccessStatusCode();
        }

        [Then(@"Submission (.*) is deleted from collection period (.*)")]
        public void ThenSubmissionIsDeleted(string submissionId, byte collectionPeriod)
        {
            AssertSubmission(submissions.SingleOrDefault(s => s.SubmissionId == submissionId && s.CollectionPeriod == collectionPeriod), false);
        }

        [Then(@"Submission (.*) is NOT deleted from collection period (.*)")]
        public void ThenSubmissionIsNotDeleted(string submissionId, byte collectionPeriod)
        {
            AssertSubmission(submissions.SingleOrDefault(s => s.SubmissionId == submissionId && s.CollectionPeriod == collectionPeriod), true);
        }

        [Then(@"Submission (.*) and Submission (.*) Both deleted")]
        public void ThenSubmissionAAndSubmissionBIsDeleted(string submissionId1, string submissionId2)
        {
            AssertSubmissions(submissions.Where(s => s.SubmissionId == submissionId1 || s.SubmissionId == submissionId2), false);
        }

        [Then(@"Submission (.*) and Submission (.*) Both NOT deleted")]
        public void ThenSubmissionAAndSubmissionBIsNotDeleted(string submissionId1, string submissionId2)
        {
            AssertSubmissions(submissions.Where(s => s.SubmissionId == submissionId1 || s.SubmissionId == submissionId2), true);
        }

        [After]
        public async Task CleanupData()
        {
            foreach (var submission in submissions)
            {
                await submissionDataContext.Database.ExecuteSqlCommandAsync
                (
                   @"DELETE FROM Payments2.Job where JobId = @JobId;
                     DELETE FROM Payments2.DataLockEventNonPayablePeriodFailures where Id = @DataLockEventNonPayablePeriodFailureId;
                     DELETE FROM Payments2.DataLockEventNonPayablePeriod where Id = @DataLockEventNonPayablePeriodId;
                     DELETE FROM Payments2.DataLockEventPayablePeriod where Id = @DataLockEventPayablePeriodId;
                     DELETE FROM Payments2.DataLockEventPriceEpisode where Id = @DataLockEventPriceEpisodeId;
                     DELETE FROM Payments2.DataLockEvent where Id = @DataLockEventId;
                     DELETE FROM Payments2.RequiredPaymentEvent where Id = @RequiredPaymentEventId;
                     DELETE FROM Payments2.FundingSourceEvent where Id = @FundingSourceEventId;
                     DELETE FROM Payments2.EarningEventPriceEpisode where Id = @EarningEventPriceEpisodeId;
                     DELETE FROM Payments2.EarningEventPeriod where Id = @EarningEventPeriodId;
                     DELETE FROM Payments2.EarningEvent where Id = @EarningEventId;",

       new SqlParameter("JobId", submission.JobModel.Id),
                     new SqlParameter("DataLockEventNonPayablePeriodFailureId", submission.DataLockEventNonPayablePeriodFailures),
                     new SqlParameter("DataLockEventNonPayablePeriodId", submission.DataLockEventNonPayablePeriodFailures),
                     new SqlParameter("DataLockEventPayablePeriodId", submission.DataLockPayablePeriod),
                     new SqlParameter("DataLockEventPriceEpisodeId", submission.DataLockEventPriceEpisode),
                     new SqlParameter("DataLockEventId", submission.DataLockEvent),
                     new SqlParameter("RequiredPaymentEventId", submission.RequiredPaymentEvent),
                     new SqlParameter("FundingSourceEventId", submission.FundingSourceEvent),
                     new SqlParameter("EarningEventPriceEpisodeId", submission.EarningEventPriceEpisode),
                     new SqlParameter("EarningEventPeriodId", submission.EarningEventPeriod),
                     new SqlParameter("EarningEventId", submission.EarningEvent));
            }
        }

        private async Task SetJobStatus(SubmissionData submission, string status)
        {
            var isValid = Enum.TryParse<JobStatus>(status, out var jobStatus);

            if (submission == null || !isValid)
            {
                Assert.Fail("Invalid spec parameters");
                return;
            }

            submission.JobModel.Status = jobStatus;
            submissionDataContext.Jobs.Update(submission.JobModel);
            await submissionDataContext.SaveChangesAsync();
        }

        private async Task SetDcJobSucceeded(SubmissionData submission, string dcJobSucceeded)
        {
            var isValid = bool.TryParse(dcJobSucceeded, out var dcJobSucceededVal);

            submission.JobModel.DcJobSucceeded = (isValid == false ? (bool?)null : dcJobSucceededVal);
            submissionDataContext.Jobs.Update(submission.JobModel);
            await submissionDataContext.SaveChangesAsync();
        }

        private void AssertSubmissions(IEnumerable<SubmissionData> submissionData, bool asExpected)
        {
            // ReSharper disable PossibleMultipleEnumeration
            if (!submissionData.Any())
            {
                Assert.Fail("Invalid spec parameters");
                return;
            }

            foreach (var submission in submissionData)
            {
                AssertSubmission(submission, asExpected);
            }
            // ReSharper restore PossibleMultipleEnumeration
        }

        private void AssertSubmission(SubmissionData submission, bool asExpected)
        {
            if (submission == null)
            {
                Assert.Fail("Invalid spec parameters");
                return;
            }

            var earningEvents = submissionDataContext.EarningEvents.Any(x => x.Id == submission.EarningEvent);
            var earningEventPeriods = submissionDataContext.EarningEventPeriods.Any(x => x.Id == submission.EarningEventPeriod);
            var earningEventPriceEpisodes = submissionDataContext.EarningEventPriceEpisodes.Any(x => x.Id == submission.EarningEventPriceEpisode);

            var fundingSourceEvents = submissionDataContext.FundingSourceEvents.Any(x => x.Id == submission.FundingSourceEvent);

            var requiredPaymentEvents = submissionDataContext.RequiredPaymentEvents.Any(x => x.Id == submission.RequiredPaymentEvent);

            var dataLockEvents = submissionDataContext.DataLockEvents.Any(x => x.Id == submission.DataLockEvent);
            var dataLockPayablePeriods = submissionDataContext.DataLockPayablePeriods.Any(x => x.Id == submission.DataLockPayablePeriod);
            var dataLockEventPriceEpisodes = submissionDataContext.DataLockEventPriceEpisodes.Any(x => x.Id == submission.DataLockEventPriceEpisode);
            var dataLockEventNonPayablePeriods = submissionDataContext.DataLockEventNonPayablePeriods.Any(x => x.Id == submission.DataLockEventNonPayablePeriod);
            var dataLockEventNonPayablePeriodFailures = submissionDataContext.DataLockEventNonPayablePeriodFailures.Any(x => x.Id == submission.DataLockEventNonPayablePeriodFailures);

            earningEvents.Should().Be(asExpected);
            earningEventPeriods.Should().Be(asExpected);
            earningEventPriceEpisodes.Should().Be(asExpected);

            fundingSourceEvents.Should().Be(asExpected);

            requiredPaymentEvents.Should().Be(asExpected);

            dataLockEvents.Should().Be(asExpected);
            dataLockPayablePeriods.Should().Be(asExpected);
            dataLockEventPriceEpisodes.Should().Be(asExpected);
            dataLockEventNonPayablePeriods.Should().Be(asExpected);
            dataLockEventNonPayablePeriodFailures.Should().Be(asExpected);
        }

        private async Task<SubmissionData> CreateSubmission(string submissionId, byte collectionPeriod, short academicYear)
        {
            var job = await AddJob(collectionPeriod, academicYear);
            // ReSharper disable once PossibleInvalidOperationException
            var jobId = job.DcJobId.Value;

            var (earningId, earningEventId) = await AddEarningEvent(jobId, collectionPeriod, academicYear);

            var (dataLockId, dataLockEventId) = await AddDataLockEvent(jobId, earningEventId, collectionPeriod, academicYear);

            var (dataLockEventNonPayablePeriodid, dataLockEventNonPayablePeriodEventId) = await AddDataLockEventNonPayablePeriod(dataLockEventId);

            return new SubmissionData
            {
                SubmissionId = submissionId,
                CollectionPeriod = collectionPeriod,
                JobModel = job,
                EarningEvent = earningId,
                EarningEventPeriod = await AddEarningEventPeriod(earningEventId),
                EarningEventPriceEpisode = await AddEarningEventPriceEpisode(earningEventId),
                FundingSourceEvent = await AddFundingSourceEvent(jobId, earningEventId, collectionPeriod, academicYear),
                RequiredPaymentEvent = await AddRequiredPaymentEvent(jobId, earningEventId, collectionPeriod, academicYear),
                DataLockEvent = dataLockId,
                DataLockPayablePeriod = await AddDataLockPayablePeriod(dataLockEventId),
                DataLockEventPriceEpisode = await AddDataLockEventPriceEpisode(dataLockEventId),
                DataLockEventNonPayablePeriod = dataLockEventNonPayablePeriodid,
                DataLockEventNonPayablePeriodFailures = await AddDataLockEventNonPayablePeriodFailures(dataLockEventNonPayablePeriodEventId)
            };
        }

        private async Task<JobModel> AddJob(byte collectionPeriod, short academicYear)
        {
            var job = new JobModel
            {
                JobType = JobType.EarningsJob,
                StartTime = DateTimeOffset.UtcNow,
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                Ukprn = ukprn,
                DcJobId = randomNumberGenerator.Next(1_000_000),
                IlrSubmissionTime = DateTime.UtcNow.AddSeconds(-10),
                Status = JobStatus.Completed,
                LearnerCount = 2,
                DcJobSucceeded = true,
                DcJobEndTime = DateTimeOffset.Now,
            };

            await submissionDataContext.Jobs.AddAsync(job);
            await submissionDataContext.SaveChangesAsync();

            return job;
        }

        private async Task<(long, Guid)> AddEarningEvent(long jobId, byte collectionPeriod, short academicYear)
        {
            var earningEvent = new EarningEvent
            {
                Ukprn = ukprn,
                LearnerUln = 123,
                EventId = Guid.NewGuid(),
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                IlrSubmissionDateTime = DateTime.Now,
                LearnerReferenceNumber = "1234",
                LearningAimReference = "1",
                EventTime = DateTimeOffset.Now,
                LearningAimFundingLineType = "LearningAimFundingLineType",
                JobId = jobId
            };

            await submissionDataContext.EarningEvents.AddAsync(earningEvent);
            await submissionDataContext.SaveChangesAsync();

            return (earningEvent.Id, earningEvent.EventId);
        }

        private async Task<long> AddEarningEventPeriod(Guid earningEventId)
        {
            var earningEventPeriod = new EarningEventPeriod
            {
                Amount = 1,
                DeliveryPeriod = 1,
                EarningEventId = earningEventId,
                TransactionType = 1
            };

            await submissionDataContext.EarningEventPeriods.AddAsync(earningEventPeriod);
            await submissionDataContext.SaveChangesAsync();

            return earningEventPeriod.Id;
        }

        private async Task<long> AddEarningEventPriceEpisode(Guid earningEventId)
        {
            var earningEventPriceEpisode = new EarningEventPriceEpisode
            {
                EarningEventId = earningEventId,
                PriceEpisodeIdentifier = "Test",
                PlannedEndDate = DateTimeOffset.Now,
                StartDate = DateTimeOffset.Now
            };

            await submissionDataContext.EarningEventPriceEpisodes.AddAsync(earningEventPriceEpisode);
            await submissionDataContext.SaveChangesAsync();

            return earningEventPriceEpisode.Id;
        }

        private async Task<long> AddFundingSourceEvent(long jobId, Guid earningEventId, byte collectionPeriod, short academicYear)
        {
            var fundingSourceEvent = new FundingSourceEvent
            {
                EventId = Guid.NewGuid(),
                EarningEventId = earningEventId,
                IlrSubmissionDateTime = DateTime.Now,
                LearnerReferenceNumber = "1234",
                LearningAimReference = "1",
                LearningAimFundingLineType = "2",
                Ukprn = ukprn,
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                JobId = jobId,
                AgreementId = "AgreementId",
                EventTime = DateTimeOffset.UtcNow,
                PriceEpisodeIdentifier = "PriceEpisodeIdentifier",
                EarningsStartDate = DateTime.Now,
            };

            await submissionDataContext.FundingSourceEvents.AddAsync(fundingSourceEvent);
            await submissionDataContext.SaveChangesAsync();

            return fundingSourceEvent.Id;
        }

        private async Task<long> AddRequiredPaymentEvent(long jobId, Guid earningEventId, byte collectionPeriod, short academicYear)
        {
            var requiredPayment = new RequiredPaymentEvent
            {
                EventId = Guid.NewGuid(),
                EarningEventId = earningEventId,
                IlrSubmissionDateTime = DateTime.Now,
                LearnerReferenceNumber = "1234",
                LearningAimReference = "1",
                LearningAimFundingLineType = "2",
                Ukprn = ukprn,
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                JobId = jobId,
                EventTime = DateTimeOffset.Now,
                EarningsStartDate = DateTime.Now,
                PriceEpisodeIdentifier = "PriceEpisodeIdentifier"
            };

            await submissionDataContext.RequiredPaymentEvents.AddAsync(requiredPayment);
            await submissionDataContext.SaveChangesAsync();

            return requiredPayment.Id;
        }

        private async Task<(long, Guid)> AddDataLockEvent(long jobId, Guid earningEventId, byte collectionPeriod, short academicYear)
        {
            var dataLockEvent = new DataLockEvent
            {
                EventId = Guid.NewGuid(),
                EarningEventId = earningEventId,
                IlrSubmissionDateTime = DateTime.Now,
                LearnerReferenceNumber = "1234",
                LearningAimReference = "1",
                LearningAimFundingLineType = "2",
                Ukprn = ukprn,
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                JobId = jobId,
                EventTime = DateTimeOffset.Now
            };

            await submissionDataContext.DataLockEvents.AddAsync(dataLockEvent);
            await submissionDataContext.SaveChangesAsync();

            return (dataLockEvent.Id, dataLockEvent.EventId);
        }

        private async Task<long> AddDataLockPayablePeriod(Guid datalockEventId)
        {
            var dataLockPayablePeriod = new DataLockPayablePeriod
            {
                TransactionType = 1,
                DeliveryPeriod = 1,
                Amount = 1,
                DataLockEventId = datalockEventId
            };

            await submissionDataContext.DataLockPayablePeriods.AddAsync(dataLockPayablePeriod);
            await submissionDataContext.SaveChangesAsync();

            return dataLockPayablePeriod.Id;
        }

        private async Task<long> AddDataLockEventPriceEpisode(Guid datalockEventId)
        {
            var dataLockEventPriceEpisode = new DataLockEventPriceEpisode
            {
                DataLockEventId = datalockEventId,
                PlannedEndDate = DateTime.Now,
                StartDate = DateTime.Now,
                PriceEpisodeIdentifier = "PriceEpisodeIdentifier"
            };

            await submissionDataContext.DataLockEventPriceEpisodes.AddAsync(dataLockEventPriceEpisode);
            await submissionDataContext.SaveChangesAsync();

            return dataLockEventPriceEpisode.Id;
        }

        private async Task<(long, Guid)> AddDataLockEventNonPayablePeriod(Guid datalockEventId)
        {
            var dataLockEventNonPayablePeriod = new DataLockEventNonPayablePeriod
            {
                DataLockEventId = datalockEventId,
                DeliveryPeriod = 1,
                Amount = 1,
                TransactionType = 1,
                DataLockEventNonPayablePeriodId = Guid.NewGuid()
            };

            await submissionDataContext.DataLockEventNonPayablePeriods.AddAsync(dataLockEventNonPayablePeriod);
            await submissionDataContext.SaveChangesAsync();

            return (dataLockEventNonPayablePeriod.Id, dataLockEventNonPayablePeriod.DataLockEventNonPayablePeriodId);
        }

        private async Task<long> AddDataLockEventNonPayablePeriodFailures(Guid dataLockEventNonPayablePeriodId)
        {
            var dataLockEvent = new DataLockEventNonPayablePeriodFailures
            {
                DataLockEventNonPayablePeriodId = dataLockEventNonPayablePeriodId,
                DataLockFailureId = 1
            };

            await submissionDataContext.DataLockEventNonPayablePeriodFailures.AddAsync(dataLockEvent);
            await submissionDataContext.SaveChangesAsync();

            return dataLockEvent.Id;
        }
    }
}
