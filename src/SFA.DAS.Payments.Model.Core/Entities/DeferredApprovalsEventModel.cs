using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class DeferredApprovalsEventModel
    {
        public long Id { get; set; }
        public DateTime EventTime { get; set; }
        public string EventType { get; set; }
        public string EventBody { get; set; }
    }
}