using System;

namespace SFA.DAS.Payments.Messages.Core.Commands
{
    public abstract class PaymentsCommand: IPaymentsMessage
    {
        public Guid CommandId { get; set; }
        public DateTimeOffset RequestTime { get; set; }
        public long JobId { get; set; }

        public DateTime SubmissionDate { get; set; }

        protected PaymentsCommand()
        {
            CommandId = Guid.NewGuid();
            RequestTime = DateTimeOffset.UtcNow;
        }
    }
}
