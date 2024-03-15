using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd.Archiving;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndArchiveJobStatusManager : IJobStatusManager
    {
        Task StartArchive(RecordPeriodEndFcsHandOverCompleteJob message);
        void StartArchiveMonitoringJob(long jobId, JobType jobType);
    }

    public class PeriodEndArchiveJobStatusManager : JobStatusManager, IPeriodEndArchiveJobStatusManager
    {
        private readonly IPeriodEndArchiveConfiguration archiveConfiguration;
        private readonly ConcurrentDictionary<long, bool> currentJobs;
        private readonly TimeSpan interval;
        private readonly IPaymentLogger logger;
        private readonly IUnitOfWorkScopeFactory scopeFactory;

        public PeriodEndArchiveJobStatusManager(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory,
            IJobServiceConfiguration configuration, IPeriodEndArchiveConfiguration archiveConfiguration) : base(logger,
            scopeFactory, configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            interval = configuration.JobStatusInterval;
            currentJobs = new ConcurrentDictionary<long, bool>();
            this.archiveConfiguration = archiveConfiguration;
        }

        public new Task Start(string partitionEndpointName, CancellationToken suppliedCancellationToken)
        {
            cancellationToken = suppliedCancellationToken;
            return Run(partitionEndpointName);
        }

        public async Task StartArchive(RecordPeriodEndFcsHandOverCompleteJob message)
        {
            try
            {
                logger.LogInfo(
                    $"Starting period end archive function. JobId: ${message.JobId}");
                var messageContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8,
                    "application/json");
                var client = new HttpClient();

                client.DefaultRequestHeaders.Add("x-functions-key", archiveConfiguration.ArchiveApiKey);

                var result = await client.PostAsync(
                    archiveConfiguration.ArchiveFunctionUrl, messageContent);

                if (!result.IsSuccessStatusCode)
                    logger.LogError(
                        $"{result.StatusCode}: HTTP error when starting period end archive function. Error: {result.Content}");
                logger.LogInfo(
                    $"Successfully called period end archive function. JobId: ${message.JobId}");
            }

            catch (Exception e)
            {
                logger.LogError(
                    $"Unable to start Period end archive function. Url: {archiveConfiguration.ArchiveFunctionUrl}. Timeout: {archiveConfiguration.ArchiveTimeout}",
                    e);
            }
        }

        public void StartArchiveMonitoringJob(long jobId, JobType jobType)
        {
            currentJobs.AddOrUpdate(jobId, false, (key, status) => status);
        }

        private async Task Run(string partitionEndpointName)
        {
            await LoadExistingJobs();
            while (!cancellationToken.IsCancellationRequested)
            {
                var tasks = currentJobs.Select(job => CheckJobStatus(partitionEndpointName, job.Key)).ToList();
                await Task.WhenAll(tasks);
                var completedJobs = currentJobs.Where(item => item.Value).ToList();
                foreach (var completedJob in completedJobs)
                {
                    logger.LogInfo(
                        $"Found completed job.  Will now stop monitoring job: {completedJob.Key}, ThreadId {Thread.CurrentThread.ManagedThreadId}, PartitionId {partitionEndpointName}");
                    if (!currentJobs.TryRemove(completedJob.Key, out _))
                        logger.LogWarning(
                            $"Couldn't remove completed job from jobs list. ThreadId {Thread.CurrentThread.ManagedThreadId}, PartitionId {partitionEndpointName},  Job: {completedJob.Key}, status: {completedJob.Value}");
                }

                await Task.Delay(interval, cancellationToken);
            }
        }

        private async Task LoadExistingJobs()
        {
            try
            {
                using (var scope = scopeFactory.Create("LoadExistingJobs"))
                {
                    var jobStorage = scope.Resolve<IJobStorageService>();
                    var jobs = await GetCurrentJobs(jobStorage);
                    foreach (var job in jobs) StartMonitoringJob(job, JobType.PeriodEndFcsHandOverCompleteJob);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to load existing jobs. Error: {e.Message}", e);
            }
        }

        private async Task CheckJobStatus(string partitionEndpointName, long jobId)
        {
            try
            {
                using (var scope =
                       scopeFactory.Create(
                           $"CheckJobStatus:{jobId}, ThreadId {Thread.CurrentThread.ManagedThreadId}, PartitionId {partitionEndpointName}"))
                {
                    try
                    {
                        var jobStatusService = GetJobStatusService(scope);
                        var finished = await jobStatusService.ManageStatus(jobId, cancellationToken);
                        await scope.Commit();
                        currentJobs[jobId] = finished;
                        logger.LogInfo($"Job: {jobId},  finished: {finished}");
                    }
                    catch (Exception ex)
                    {
                        scope.Abort();
                        logger.LogWarning(
                            $"Failed to update job status for job: {jobId}, ThreadId {Thread.CurrentThread.ManagedThreadId}, PartitionId {partitionEndpointName} Error: {ex.Message}. {ex}");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(
                    $"Failed to create or abort the scope of the state manager transaction for: {jobId}, ThreadId {Thread.CurrentThread.ManagedThreadId}, PartitionId {partitionEndpointName} Error: {e.Message}",
                    e);
                throw;
            }
        }

        public override IJobStatusService GetJobStatusService(IUnitOfWorkScope scope)
        {
            return scope.Resolve<IPeriodEndArchiveStatusService>();
        }

        public override async Task<List<long>> GetCurrentJobs(IJobStorageService jobStorage)
        {
            return await jobStorage.GetCurrentPeriodEndFcsHandoverJobs(cancellationToken);
        }
    }
}