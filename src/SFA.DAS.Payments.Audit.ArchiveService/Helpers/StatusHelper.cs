using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;

namespace SFA.DAS.Payments.Audit.ArchiveService.Helpers
{
    public static class StatusHelper
    {
        public static async Task UpdateCurrentJobStatus(IDurableEntityClient entityClient, EntityId entityId,
            RunInformation runInformation)
        {
            await entityClient.SignalEntityAsync(entityId, "add", runInformation);
        }

        public static async Task<RunInformation> GetCurrentJobs(IDurableEntityClient entityClient)
        {
            var entityId = new EntityId(nameof(HandleCurrentJobId.Handle),
                HandleCurrentJobId.PeriodEndArchiveEntityName);
            var stateResponse = await entityClient.ReadEntityStateAsync<RunInformation>(entityId);
            return stateResponse.EntityExists ? stateResponse.EntityState : null;
        }
    }
}