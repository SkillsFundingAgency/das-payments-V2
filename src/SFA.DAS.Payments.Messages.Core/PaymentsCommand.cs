using System;

namespace SFA.DAS.Payments.Messages.Core
{
    public abstract class PaymentsCommand: IPaymentsMessage
    {
        public DateTimeOffset RequestTime { get; set; }
        public string JobId { get; set; }

        protected PaymentsCommand()
        {
            RequestTime = DateTimeOffset.UtcNow;
        }
    }
}
