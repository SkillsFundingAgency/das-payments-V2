using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.FundingSource.Model.Entities
{
    public class LevyAccountEntity
    {
        public long AccountId { get; set; }
        public string AccountHashId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public long SequenceId { get; set; }
        public bool IsLevyPayer { get; set; }
        public decimal TransferAllowance { get; set; }

    }
}