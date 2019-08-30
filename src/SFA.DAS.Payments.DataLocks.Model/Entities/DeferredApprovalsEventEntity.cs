using System;

namespace SFA.DAS.Payments.DataLocks.Model.Entities
{
    public class DeferredApprovalsEventEntity
    {
        public long Id { get; set; }
        public DateTime EventTime { get; set; }
        public object ApprovalsEvent { get; set; }
    }
}