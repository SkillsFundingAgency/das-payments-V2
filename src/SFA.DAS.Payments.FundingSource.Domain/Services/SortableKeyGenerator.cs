using System;
using SFA.DAS.Payments.FundingSource.Domain.Interface;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SortableKeyGenerator : ISortableKeyGenerator
    {
        public string Generate(decimal amountDue, int priority, long uln, Guid uniqueId)
        {
            return string.Concat(amountDue < 0 ? "1" : "9", "-",
                priority.ToString("000000"), "-",
                uln, "-",
                uniqueId.ToString("N"));
        }
    }
}
