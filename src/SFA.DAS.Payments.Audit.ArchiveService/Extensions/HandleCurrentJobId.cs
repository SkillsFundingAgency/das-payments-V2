using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.ArchiveService.Extensions
{
    public static class HandleCurrentJobId
    {
        public const string PeriodEndArchiveEntityName = "CurrentPeriodEndArchiveJobId";

        [FunctionName(nameof(Handle))]
        public static void Handle([EntityTrigger] IDurableEntityContext ctx)
        {
            var currentValue = ctx.GetState<ArchiveRunInformation>();
            switch (ctx.OperationName.ToLowerInvariant())
            {
                case "add":
                    var newJobId = ctx.GetInput<ArchiveRunInformation>();
                    ctx.SetState(newJobId);
                    break;
                case "reset":
                    ctx.SetState(new ArchiveRunInformation());
                    break;
                case "get":
                    ctx.Return(currentValue);
                    break;
            }
        }
    }
}