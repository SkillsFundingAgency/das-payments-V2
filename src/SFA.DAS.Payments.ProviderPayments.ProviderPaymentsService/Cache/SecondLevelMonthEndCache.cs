using System.Collections.Concurrent;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Cache
{
    public interface ISecondLevelMonthEndCache
    {
        ConcurrentDictionary<MonthEndDetails, long> MonthEndDetailsStore { get; }
        ConcurrentDictionary<MonthEndDetails, long> MonthEndDetailsChecked { get; }
    }

    public class SecondLevelMonthEndCache : ISecondLevelMonthEndCache
    {
        public ConcurrentDictionary<MonthEndDetails, long> MonthEndDetailsStore { get; } = new ConcurrentDictionary<MonthEndDetails, long>();
        public ConcurrentDictionary<MonthEndDetails, long> MonthEndDetailsChecked { get; } = new ConcurrentDictionary<MonthEndDetails, long>();
    }
}
