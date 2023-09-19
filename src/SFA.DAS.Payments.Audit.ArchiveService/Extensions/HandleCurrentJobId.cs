using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SFA.DAS.Payments.Audit.ArchiveService.Extensions
{
    public class RunInformation
    {
        public string JobId { get; set; }
        public string Status { get; set; }
    }

    public static class HandleCurrentJobId
    {
        public const string PeriodEndArchiveEntityName = "CurrentPeriodEndArchiveJobId";

        [FunctionName(nameof(Handle))]
        public static void Handle([EntityTrigger] IDurableEntityContext ctx)
        {
            var currentValue = ctx.GetState<RunInformation>();
            switch (ctx.OperationName.ToLowerInvariant())
            {
                case "add":
                    var newJobId = ctx.GetInput<RunInformation>();
                    ctx.SetState(newJobId);
                    break;
                case "reset":
                    ctx.SetState(new RunInformation());
                    break;
                case "get":
                    ctx.Return(currentValue);
                    break;
            }
        }
    }
}