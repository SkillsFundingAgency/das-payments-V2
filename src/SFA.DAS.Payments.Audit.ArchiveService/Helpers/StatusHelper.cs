using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.ArchiveService.Helpers
{
    public static class StatusHelper
    {
        public enum ArchiveStatus
        {
            InProgress,
            Queued,
            Completed,
            Failed
        }

        public static EntityId GetEntityId()
        {
            return new EntityId(nameof(HandleCurrentJobId.Handle),
                HandleCurrentJobId.PeriodEndArchiveEntityName);
        }

        public static async Task UpdateCurrentJobStatus(IDurableEntityClient entityClient,
            ArchiveRunInformation runInformation)
        {
            var entityId = GetEntityId();
            await entityClient.SignalEntityAsync(entityId, "add", runInformation);
        }

        public static async Task ClearCurrentStatus(IDurableEntityClient entityClient, IPaymentLogger log)
        {
            log.LogInfo("StatusHelper.ClearCurrentStatus: Clearing down previous archive job");

            var previousRun = await GetCurrentJobs(entityClient);
            if (previousRun != null)
            {
                log.LogInfo(
                    $"StatusHelper.ClearCurrentStatus: Previous JobId: {previousRun.JobId}, JobStatus: {previousRun.Status}");
            }

            var entityId = new EntityId(nameof(HandleCurrentJobId.Handle),
                HandleCurrentJobId.PeriodEndArchiveEntityName);

            await entityClient.SignalEntityAsync(entityId, "add", new ArchiveRunInformation
            {
                JobId = string.Empty,
                InstanceId = string.Empty,
                Status = string.Empty
            });
            var currentRun = await GetCurrentJobs(entityClient);

            log.LogInfo(
                $"StatusHelper.ClearCurrentStatus: Current JobId: {currentRun.JobId}, JobStatus: {currentRun.Status}");
        }

        public static async Task<ArchiveRunInformation> GetCurrentJobs(IDurableEntityClient entityClient)
        {
            var entityId = GetEntityId();
            var stateResponse = await entityClient.ReadEntityStateAsync<ArchiveRunInformation>(entityId);
            return stateResponse.EntityExists ? stateResponse.EntityState : new ArchiveRunInformation();
        }
    }
}