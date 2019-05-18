using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ISortableKeyGenerator
    {
        string Generate(decimal amountDue, int priority, long uln, DateTime startDate, bool isTransfer);
    }
}
