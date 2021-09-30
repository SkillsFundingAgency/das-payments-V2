using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model
{
    public class ProviderNegativeEarningsTotal
    {
        public long Ukprn { get; set; }
        public ContractType ContractType { get; set; }
        public decimal NegativeEarningsTotal { get; set; }
    }
}
