using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public class RecordEarningsJobAdditionalMessages : JobsCommand
    {
        public List<GeneratedMessage> GeneratedMessages { get; set; }
        public RecordEarningsJobAdditionalMessages()
        {
            GeneratedMessages = new List<GeneratedMessage>();
        }
    }
}