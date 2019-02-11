using System;
using SFA.DAS.Payments.FundingSource.Domain.Interface;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SortableKeyGenerator : IGenerateSortableKeys
    {
        public string Generate(decimal amountDue, int priority, DateTime dateAgreed, long uln)
        {
            return string.Concat(amountDue < 0 ? "1" : "9", "-",
                priority.ToString("000000"), "-",
                dateAgreed.ToString("yyyyMMddhhmm"), "-",
                uln);
        }
    }
}
