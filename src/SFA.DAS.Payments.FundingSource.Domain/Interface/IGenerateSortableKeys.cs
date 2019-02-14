using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface IGenerateSortableKeys
    {
        string Generate(decimal amountDue, int priority, DateTime dateAgreed, long uln, Guid uniqueId);
    }
}
