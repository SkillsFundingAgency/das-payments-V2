using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public class InProgressMessage
    {
        public long JobId { get; set; }
        public Guid MessageId { get; set; }
        public string MessageName { get; set; }
    }
}