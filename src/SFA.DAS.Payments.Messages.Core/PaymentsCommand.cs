using System;

namespace SFA.DAS.Payments.Messages.Core
{
    public abstract class PaymentsCommand: IPaymentsMessage
    {
        public DateTimeOffset RequestTime { get; }

        protected PaymentsCommand()
        {
            RequestTime = DateTimeOffset.UtcNow;
        }
    }
}
