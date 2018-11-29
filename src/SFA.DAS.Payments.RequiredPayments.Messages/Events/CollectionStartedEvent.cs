using System.Collections.Generic;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CollectionStartedEvent : IPaymentsMessage
    {
        public long JobId { get; set;  }
        public IReadOnlyCollection<string> ApprenticeshipKeys { get; set; }
    }
}
