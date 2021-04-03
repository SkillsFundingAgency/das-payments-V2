using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public class RecordBatchOfJobMessageProcessingStatus : JobsCommand
    {
        public List<RecordJobMessageProcessingStatus> JobMessageProcessingStatuses { get; set; } =
            new List<RecordJobMessageProcessingStatus>();
    }
}