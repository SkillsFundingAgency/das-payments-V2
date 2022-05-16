using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FPA.Messages.Events
{
    public class FlexiPaymentsReadyForProcessingEvent
    {
        public long EmployerAccountId { get; set; }
        public long JobId { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
    }
}
