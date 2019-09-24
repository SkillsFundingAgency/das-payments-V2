using System;

namespace SFA.DAS.Payments.DataLocks.Model.Entities
{
    public class LegacyDataLockEventError
    {
        public Guid DataLockEventId { get; set; }
        public string ErrorCode { get; set; }
        public string SystemDescription { get; set; } 
    }
}