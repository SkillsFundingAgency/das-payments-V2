using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Employer
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
