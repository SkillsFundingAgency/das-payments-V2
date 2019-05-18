using System;
using SFA.DAS.Payments.FundingSource.Domain.Interface;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SortableKeyGenerator : ISortableKeyGenerator
    {
        public string Generate(decimal amountDue, int priority, long uln, DateTime startDate, bool isTransfer)
        {
            var paymentTypeOrder = amountDue < 0
                ? "1"
                : isTransfer
                    ? "2"
                    : "9";

            return string.Concat(paymentTypeOrder, 
                "-",
                isTransfer ? "000000" :   priority.ToString("000000"), 
                "-",
                startDate.Date.ToString("s"),
                "-",
                uln);
        }
    }
}
