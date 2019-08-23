using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data
{
    public interface IJobsDataContext
    {
        Task SaveNewJob(JobModel jobDetails, List<JobStepModel> jobSteps, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> GetJobIdFromDcJobId(long dcJobId);
        Task<JobModel> GetJobByDcJobId(long dcJobId);
        Task SaveJobSteps(List<JobStepModel> jobSteps);
        Task<List<JobStepModel>> GetJobSteps(List<Guid> messageIds);
        Task<Dictionary<JobMessageStatus, int>> GetJobStepsStatus(long jobId);
        Task<DateTimeOffset?> GetLastJobStepEndTime(long jobId);
        Task<JobModel> GetJob(long jobId);
        Task SaveJobStatus(long jobId, JobStatus status, DateTimeOffset endTime);
        Task<List<JobModel>> GetInProgressJobs();
        Task UpdateJob(JobModel job, CancellationToken cancellationToken = default(CancellationToken));

    }

    public class JobsDataContext : DbContext, IJobsDataContext
    {
        private readonly string connectionString;
        public virtual DbSet<JobModel> Jobs { get; set; }
        public virtual DbSet<JobStepModel> JobSteps { get; set; }

        public JobsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Jobs");
            modelBuilder.ApplyConfiguration(new JobModelConfiguration());
            modelBuilder.ApplyConfiguration(new JobMessageStartedModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task SaveNewJob(JobModel jobDetails, List<JobStepModel> jobSteps, CancellationToken cancellationToken = default(CancellationToken))
        {
            Jobs.Add(jobDetails);
            await SaveChangesAsync(cancellationToken);
            jobSteps.ForEach(step => step.JobId = jobDetails.Id);
            JobSteps.AddRange(jobSteps);
            await SaveChangesAsync(cancellationToken);
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

        public async Task<Dictionary<JobMessageStatus, int>> GetJobStepsStatus(long jobId)
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
            return await Jobs.FirstOrDefaultAsync(job => job.DcJobId == dcJobId);
        }

        public async Task SaveJobStatus(long jobId, JobStatus status, DateTimeOffset endTime)
        {
            var job = await GetJob(jobId) ?? throw new ArgumentException($"Job not found: {jobId}");
            job.EndTime = endTime;
            job.Status = status;
            await SaveChangesAsync();
        }
    }
}