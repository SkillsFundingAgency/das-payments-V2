using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ITransferPaymentProcessor
    {
        IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment);
    }
}