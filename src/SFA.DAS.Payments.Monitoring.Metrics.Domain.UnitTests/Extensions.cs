using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests
{
    public static class Extensions
    {
        public static decimal GetTotal<T>(this List<T> amounts, ContractType contractType) where T : TransactionTypeAmountsByContractType
        {
            return amounts.FirstOrDefault(x => x.ContractType == contractType)?.Total ?? 0;
        }
    }
}