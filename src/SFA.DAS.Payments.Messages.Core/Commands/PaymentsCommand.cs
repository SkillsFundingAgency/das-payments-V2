using System;

namespace SFA.DAS.Payments.Messages.Core.Commands
{
    public abstract class PaymentsCommand: IPaymentsMessage
    {
        public DateTimeOffset RequestTime { get; set; }
        public long JobId { get; set; }

        public DateTime SubmissionDate { get; set; }

        protected PaymentsCommand()
        {
            RequestTime = DateTimeOffset.UtcNow;
        }
    }
}
