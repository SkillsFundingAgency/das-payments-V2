using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Payment
    {
        public string CollectionPeriod { get; set; }
        public string DeliveryPeriod { get; set; }
        public decimal OnProgramme { get; set; }
        public decimal Completion { get; set; }
        public decimal Balancing { get; set; }

        public IDictionary<IncentiveType, decimal> IncentiveValues { get; set; } = new Dictionary<IncentiveType, decimal>();
    }
}
