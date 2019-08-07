using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class EmployerChangedProviderPriority : IEvent
    {
        public long EmployerAccountId { get; set; }
        public List<long> OrderedProviders { get; set; }
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }

        public EmployerChangedProviderPriority()
        {
            EventId = Guid.NewGuid();
            EventTime = DateTimeOffset.UtcNow;
        }
    }
}
