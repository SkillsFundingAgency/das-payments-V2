﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data
{
    public interface IJobsDataContext
    {
        Task SaveNewJob(JobModel jobDetails, CancellationToken cancellationToken = default(CancellationToken));
        Task<long?> GetNonFailedDcJobId(JobType jobType, short academicYear, byte collectionPeriod);
        Task<long> GetJobIdFromDcJobId(long dcJobId);
        Task<JobModel> GetJobByDcJobId(long dcJobId);
        Task SaveJobSteps(List<JobStepModel> jobSteps);
        Task<List<JobStepModel>> GetJobSteps(List<Guid> messageIds);
        Task<Dictionary<JobStepStatus, int>> GetJobStepsStatus(long jobId);
        Task<DateTimeOffset?> GetLastJobStepEndTime(long jobId);
        Task<JobModel> GetJob(long jobId);
        Task SaveJobStatus(long dcJobId, JobStatus status, DateTimeOffset endTime, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<JobModel>> GetInProgressJobs();
        Task SaveDataLocksCompletionTime(long jobId, DateTimeOffset endTime, CancellationToken cancellationToken);
        Task SaveDcSubmissionStatus(long jobId, bool succeeded, CancellationToken cancellationToken);
        Task<List<OutstandingJobResult>> GetOutstandingOrTimedOutJobs(JobModel job, CancellationToken cancellationToken);
        List<long?> DoSubmissionSummariesExistForJobs(List<OutstandingJobResult> jobs);
        Task<List<InProgressJobAverageJobCompletionTime>> GetAverageJobCompletionTimesForInProgressJobs(List<long?> inProgressJobsUkprnList, CancellationToken cancellationToken);
    }

    public class JobsDataContext : DbContext, IJobsDataContext
    {
        private readonly string connectionString;
        public virtual DbSet<JobModel> Jobs { get; set; }
        public virtual DbSet<JobStepModel> JobSteps { get; set; }
        public virtual DbSet<SubmissionSummaryModel> SubmissionSummaries { get; set; }

        private const int ValidStartTimeOffsetMinutes = -150;

        public JobsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new JobModelConfiguration());
            modelBuilder.ApplyConfiguration(new JobStepModelConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPaymentsModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockCountsModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningsModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task SaveNewJob(JobModel jobDetails, CancellationToken cancellationToken = default(CancellationToken))
        {
            var job = await Jobs.AsNoTracking().FirstOrDefaultAsync(storedJob => storedJob.DcJobId == jobDetails.DcJobId, cancellationToken);
            
            if (job != null) return;
            
            Jobs.Add(jobDetails);

            await SaveChangesAsync(cancellationToken);
        }

        public async Task<long?> GetNonFailedDcJobId(JobType jobType, short academicYear, byte collectionPeriod)
        {
            return await Jobs
                .Where(x => x.JobType == jobType &&
                            x.AcademicYear == academicYear &&
                            x.CollectionPeriod == collectionPeriod &&
                            (x.Status == JobStatus.Completed ||
                            x.Status == JobStatus.InProgress))
                .Select(job => job.DcJobId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateJob(JobModel job, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (job.Id < 1)
                Jobs.Add(job);
            else
                Jobs.Attach(job).State = EntityState.Modified;
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<long> GetJobIdFromDcJobId(long dcJobId)
        {
            return await Jobs.Where(job => job.DcJobId == dcJobId)
                .Select(job => job.Id)
                .FirstOrDefaultAsync();
        }

        public async Task SaveJobSteps(List<JobStepModel> jobSteps)
        {
            JobSteps.AddRange(jobSteps.Where(step => step.Id == 0));
            await SaveChangesAsync();
        }

        public async Task<List<JobStepModel>> GetJobSteps(List<Guid> messageIds)
        {
            return await JobSteps.Where(step => messageIds.Contains(step.MessageId)).ToListAsync();
        }

        public async Task<Dictionary<JobStepStatus, int>> GetJobStepsStatus(long jobId)
        {
            return await JobSteps.Where(step => step.JobId == jobId)
                .GroupBy(step => step.Status)
                .Select(grp => new
                {
                    grp.Key,
                    Count = grp.Count()
                })
                .ToDictionaryAsync(item => item.Key, item => item.Count);
        }

        public async Task<List<JobModel>> GetInProgressJobs()
        {
            return await Jobs
                .Where(job => job.Status == JobStatus.InProgress)
                .ToListAsync();
        }

        public async Task SaveDataLocksCompletionTime(long dcJobId, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            var job = await Jobs.FirstOrDefaultAsync(storedJob => storedJob.DcJobId == dcJobId, cancellationToken) ??
                      throw new InvalidOperationException($"Job not found: {dcJobId}");

            job.DataLocksCompletionTime = endTime;
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task SaveDcSubmissionStatus(long dcJobId, bool succeeded, CancellationToken cancellationToken)
        {
            var job = await Jobs.FirstOrDefaultAsync(storedJob => storedJob.DcJobId == dcJobId, cancellationToken) ??
                      throw new InvalidOperationException($"Job not found: {dcJobId}");

            job.DcJobEndTime = DateTimeOffset.UtcNow;
            job.DcJobSucceeded = succeeded;

            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<DateTimeOffset?> GetLastJobStepEndTime(long jobId)
        {
            var time = await JobSteps
                .Where(step => step.JobId == jobId && step.EndTime != null)
                .Select(step => step.EndTime)
                .OrderByDescending(endTime => endTime)
                .FirstOrDefaultAsync();
            return time;
        }

        public async Task<JobModel> GetJob(long jobId)
        {
            return await Jobs.FirstOrDefaultAsync(job => job.Id == jobId);
        }

        public async Task<JobModel> GetJobByDcJobId(long dcJobId)
        {
            return await Jobs.AsNoTracking().FirstOrDefaultAsync(job => job.DcJobId == dcJobId);
        }

        public async Task SaveJobStatus(long dcJobId, JobStatus status, DateTimeOffset endTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            var job = await Jobs.FirstOrDefaultAsync(storedJob => storedJob.DcJobId == dcJobId, cancellationToken)
                      ?? throw new ArgumentException($"Job not found: {dcJobId}");
            job.EndTime = endTime;
            job.Status = status;
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<OutstandingJobResult>> GetOutstandingOrTimedOutJobs(JobModel job, CancellationToken cancellationToken)
        {
            var latestValidStartTime = job.StartTime.AddMinutes(ValidStartTimeOffsetMinutes);

            return await Jobs.Where(x =>
                    x.AcademicYear == job.AcademicYear &&
                    x.CollectionPeriod == job.CollectionPeriod &&
                    x.DcJobId != job.DcJobId &&
                    x.JobType == JobType.EarningsJob &&
                    x.StartTime > latestValidStartTime)
                .Select(x => new OutstandingJobResult { DcJobId = x.DcJobId, DcJobSucceeded = x.DcJobSucceeded, JobStatus = x.Status, EndTime = x.EndTime, StartTime = x.StartTime, IlrSubmissionTime = x.IlrSubmissionTime, Ukprn = x.Ukprn })
                .ToListAsync(cancellationToken);
        }

        public List<long?> DoSubmissionSummariesExistForJobs(List<OutstandingJobResult> jobs)
        {
            var jobIdsToCheck = jobs
                .GroupBy(x => x.Ukprn)
                .Select(grp => grp.OrderByDescending(x => x.IlrSubmissionTime).First().DcJobId); //get the DCJobId that pertains to the latest submission for each ukprn grouping

            //Select jobId where SubmissionSummaries does not have the same jobId
            return jobIdsToCheck.Where(x => !SubmissionSummaries.Any(y => y.JobId == x)).ToList();
        }

        public async Task<List<InProgressJobAverageJobCompletionTime>> GetAverageJobCompletionTimesForInProgressJobs(List<long?> inProgressJobsUkprnList, CancellationToken cancellationToken)
        {
            return await Jobs.Where(x =>
                    x.JobType == JobType.EarningsJob &&
                    x.DcJobSucceeded == true &&
                    (x.Status == JobStatus.Completed || x.Status == JobStatus.CompletedWithErrors) &&
                    inProgressJobsUkprnList.Contains(x.Ukprn))
                .Select(j => new
                {
                    j.Ukprn,
                    AverageJobCompletionTime = EF.Functions.DateDiffMillisecond(j.StartTime, j.EndTime ?? DateTimeOffset.UtcNow)
                })
                .GroupBy(g => g.Ukprn)
                .Select(x => new InProgressJobAverageJobCompletionTime
                {
                    Ukprn = x.Key.Value,
                    AverageJobCompletionTime = x.Average(item => item.AverageJobCompletionTime) + (x.Average(item => item.AverageJobCompletionTime) * 0.20)
                })
                .ToListAsync(cancellationToken);
        }
    }
}