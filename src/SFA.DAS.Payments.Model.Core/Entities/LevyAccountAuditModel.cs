using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class LevyAccountAuditModel
    {
        public long AccountId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public decimal LevyAccountBalance { get; set; }
        public decimal RemainingTransferAllowance { get; set; } 
        public bool IsLevyPayer { get; set; }
    }
}
