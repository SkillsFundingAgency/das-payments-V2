using System.Collections.Concurrent;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache
{
    public interface ISecondLevelMonthEndCache
    {
        ConcurrentDictionary<MonthEndDetails, int> MonthEndDetailsStore { get; }
        ConcurrentDictionary<MonthEndDetails, int> MonthEndDetailsChecked { get; }
    }

    public class SecondLevelMonthEndCache : ISecondLevelMonthEndCache
    {
        public ConcurrentDictionary<MonthEndDetails, int> MonthEndDetailsStore { get; } = new ConcurrentDictionary<MonthEndDetails, int>();
        public ConcurrentDictionary<MonthEndDetails, int> MonthEndDetailsChecked { get; } = new ConcurrentDictionary<MonthEndDetails, int>();
    }
}
