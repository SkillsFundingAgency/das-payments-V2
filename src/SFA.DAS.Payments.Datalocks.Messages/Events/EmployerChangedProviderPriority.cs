using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class EmployerChangedProviderPriority
    {
        public long EmployerAccountId { get; set; }
        public List<long> OrderedProviders { get; set; }
    }
}
