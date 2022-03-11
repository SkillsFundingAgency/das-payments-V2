using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class LevyAccountAuditModel
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public decimal SourceLevyAccountBalance { get; set; }
        public decimal AdjustedLevyAccountBalance { get; set; }
        public decimal SourceTransferAllowance { get; set; } 
        public decimal AdjustedTransferAllowance { get; set; } 
        public bool IsLevyPayer { get; set; }
    }
}
