using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests
{
    public static class Extensions
    {
        public static decimal GetTotal(this List<TransactionTypeAmounts> amounts, ContractType contractType)
        {
            return amounts.FirstOrDefault(x => x.ContractType == contractType)?.Total ?? 0;
        }
    }
}