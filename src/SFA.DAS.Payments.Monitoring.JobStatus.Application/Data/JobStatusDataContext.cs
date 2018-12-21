using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Configuration;
using SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data
{
    public interface IJobStatusDataContext
    {
        Task<JobModel> SaveNewProviderEarningsJob(
            (long DcJobId, DateTimeOffset StartTime, byte CollectionPeriod, short CollectionYear,
                long Ukprn, DateTime ilrSubmissionTime, List<(DateTimeOffset StartTime,
                    Guid MessageId)> GeneratedMessages) jobDetails, CancellationToken cancellationToken = default(CancellationToken));

        Task<JobStepModel> StoreMessageProcessingStatus(
            (long DcJobId, Guid MessageId, DateTimeOffset EndTime, JobStepStatus Status, string messageName, List<(DateTimeOffset StartTime,
                Guid MessageId)> GeneratedMessages) messageProcessingDetails, CancellationToken cancellationToken = default(CancellationToken));
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

        public async Task<JobModel> SaveNewProviderEarningsJob(
            (long DcJobId, DateTimeOffset StartTime, byte CollectionPeriod, short CollectionYear, long Ukprn, DateTime ilrSubmissionTime, List<(DateTimeOffset StartTime, Guid MessageId)> GeneratedMessages) jobDetails,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var job = new JobModel
            {
                StartTime = jobDetails.StartTime,
                Status = Data.Model.JobStatus.InProgress
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Jobs.Add(job);
                await SaveChangesAsync(cancellationToken);
                var providerEarningsJob = new ProviderEarningsJobModel
                {
                    Id = job.Id,
                    CollectionPeriod = jobDetails.CollectionPeriod,
                    CollectionYear = jobDetails.CollectionYear,
                    Ukprn = jobDetails.Ukprn,
                    IlrSubmissionTime = jobDetails.ilrSubmissionTime,
                    DcJobId = jobDetails.DcJobId,
                };
                ProviderEarnings.Add(providerEarningsJob);

                (job.JobEvents ?? (job.JobEvents = new List<JobStepModel>())).AddRange(jobDetails.GeneratedMessages.Select(msg => new JobStepModel
                {
                    Job = job,
                    StartTime = msg.StartTime,
                    Status = JobStepStatus.Queued,
                    MessageId = msg.MessageId
                }));

                await SaveChangesAsync(cancellationToken);
                scope.Complete();
            }

            return job;
        }

        public async Task<JobStepModel> StoreMessageProcessingStatus(
            (long DcJobId, Guid MessageId, DateTimeOffset EndTime, JobStepStatus Status, string messageName, List<(DateTimeOffset StartTime, Guid MessageId)> GeneratedMessages)
                messageProcessingDetails, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var jobId = await ProviderEarnings
                    .Where(pe => pe.DcJobId == messageProcessingDetails.DcJobId)
                    .Select(pe => pe.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (jobId == 0)
                    throw new InvalidOperationException($"Job not found. DcJob id: {messageProcessingDetails.DcJobId}");

                var jobStep = await JobSteps.Where(js => js.JobId == jobId && 
                                                        js.MessageId == messageProcessingDetails.MessageId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (jobStep == null)
                {
                    jobStep = new JobStepModel
                    {
                        JobId = jobId,
                        MessageId = messageProcessingDetails.MessageId,
                    };
                    JobSteps.Add(jobStep);
                }

                jobStep.EndTime = messageProcessingDetails.EndTime;
                jobStep.Status = messageProcessingDetails.Status;

                var generatedMessages = messageProcessingDetails.GeneratedMessages
                    .GroupJoin(JobSteps.Where(js => js.JobId == jobId),
                        gm => gm.MessageId,
                        js => js.MessageId,
                        (generatedMessage, items) => new
                            {GeneratedMessage = generatedMessage, JobStep = items.FirstOrDefault()})
                    .ToList();

                generatedMessages.ForEach(gm =>
                {
                    var generatedMessage = gm.JobStep;
                    if (generatedMessage == null)
                    {
                        generatedMessage = new JobStepModel
                        {
                            JobId = jobId,
                            MessageId = gm.GeneratedMessage.MessageId,
                            Status = JobStepStatus.Queued,
                            MessageName = jobStep.MessageName
                        };
                        JobSteps.Add(generatedMessage);
                    }

                    generatedMessage.StartTime = gm.GeneratedMessage.StartTime;
                    generatedMessage.ParentMessageId = messageProcessingDetails.MessageId;
                });
            
                await SaveChangesAsync(cancellationToken);
                scope.Complete();
                return jobStep;
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
            jobSteps.AddRange(jobSteps);
            return SaveChangesAsync();
        }

        public async Task<JobStepModel> GetJobStep(Guid messageId)
        {
            return await JobSteps.FirstOrDefaultAsync(jobStep => jobStep.MessageId == messageId);
        }
    }
}