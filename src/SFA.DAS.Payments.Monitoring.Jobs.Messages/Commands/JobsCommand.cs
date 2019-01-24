using System;
using SFA.DAS.Payments.Messages.Core.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
    public abstract class JobsCommand: JobsMessage, IPaymentsCommand
    {
        public Guid CommandId { get; set; }
        public DateTimeOffset RequestTime { get; set; }
        protected JobsCommand()
        {
            CommandId = Guid.NewGuid();
            RequestTime = DateTimeOffset.UtcNow;
        }
    }
}