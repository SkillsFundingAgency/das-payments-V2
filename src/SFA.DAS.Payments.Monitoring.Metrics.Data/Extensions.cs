using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public static class Extensions
    {
        public static List<List<T>> SplitIntoBatchesOf<T>(this List<T> items, int batchSize) =>
            items
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / batchSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
    }
}
