using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Configuration;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data
{
    public interface IJobStatusDataContext
    {
        //DbSet<JobModel> Payment { get; set; }
        Task SaveNewJob(JobModel job, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> GetJobIdFromDcJobId(long dcJobId);
        Task SaveJobSteps(List<JobStepModel> jobSteps);
        Task<JobStepModel> GetJobStep(Guid messageId);
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

        public async Task SaveNewJob(JobModel job, CancellationToken cancellationToken = default(CancellationToken))
        {
            Jobs.Add(job);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<long> GetJobIdFromDcJobId(long dcJobId)
        {
            return await ProviderEarnings.Where(providerEarnings => providerEarnings.DcJobId == dcJobId)
                .Select(providerEarnings => providerEarnings.Id)
                .FirstOrDefaultAsync();
        }

        public Task SaveJobSteps(List<JobStepModel> jobSteps)
        {
            jobSteps.AddRange(jobSteps);
            return SaveChangesAsync();
        }

        public async Task<JobStepModel> GetJobStep(Guid messageId)
        {
            return await JobSteps.FirstOrDefaultAsync(jobStep => jobStep.MessageId == messageId);
        }
    }
}