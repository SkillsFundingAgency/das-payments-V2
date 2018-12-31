using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Data.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Data
{
    public interface IJobStatusDataContext
    {
        Task SaveNewProviderEarningsJob(JobModel jobDetails, ProviderEarningsJobModel providerEarningsJobDetails, List<JobStepModel> jobSteps, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> GetJobIdFromDcJobId(long dcJobId);
        Task SaveJobSteps(List<JobStepModel> jobSteps);
        Task<List<JobStepModel>> GetJobSteps(List<Guid> messageIds);
        Task<Dictionary<JobStepStatus, int>> GetJobStepsStatus(long jobId);
        Task<DateTimeOffset> GetLastJobStepEndTime(long jobId);
        Task<JobModel> GetJob(long jobId);
        Task SaveJobStatus(long jobId, JobStatus status, DateTimeOffset endTime);
    }

    public class JobStatusDataContext : DbContext, IJobStatusDataContext
    {
        private readonly string connectionString;
        public virtual DbSet<JobModel> Jobs { get; set; }
        public virtual DbSet<ProviderEarningsJobModel> ProviderEarnings { get; set; }
        public virtual DbSet<JobStepModel> JobSteps { get; set; }

        public JobStatusDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new JobModelConfiguration());
            modelBuilder.ApplyConfiguration(new JobStepModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderEarningsJobModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task SaveNewProviderEarningsJob(JobModel jobDetails, ProviderEarningsJobModel providerEarningsJobDetails, List<JobStepModel> jobSteps, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Jobs.Add(jobDetails);
                await SaveChangesAsync(cancellationToken);
                providerEarningsJobDetails.Id = jobDetails.Id;
                ProviderEarnings.Add(providerEarningsJobDetails);
                jobSteps.ForEach(step => step.JobId = jobDetails.Id);
                jobSteps.AddRange(jobSteps);
                await SaveChangesAsync(cancellationToken);
                scope.Complete();
            }
        }

        public async Task<long> GetJobIdFromDcJobId(long dcJobId)
        {
            return await ProviderEarnings.Where(providerEarnings => providerEarnings.DcJobId == dcJobId)
                .Select(providerEarnings => providerEarnings.Id)
                .FirstOrDefaultAsync();
        }

        public Task SaveJobSteps(List<JobStepModel> jobSteps)
        {
            jobSteps.AddRange(jobSteps.Where(step => step.Id == 0));
            return SaveChangesAsync();
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
                    grp.Key, Count = grp.Count()
                })
                .ToDictionaryAsync(item => item.Key, item => item.Count);
        }

        public async Task<DateTimeOffset> GetLastJobStepEndTime(long jobId)
        {
            var time = await JobSteps
                .Where(step => step.JobId == jobId && step.EndTime!=null)
                .Select(step => step.EndTime)
                .OrderByDescending(endTime => endTime)
                .FirstOrDefaultAsync();
            return time.Value;
        }

        public async Task<JobModel> GetJob(long jobId)
        {
            return await Jobs.FirstOrDefaultAsync(job => job.Id == jobId);
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