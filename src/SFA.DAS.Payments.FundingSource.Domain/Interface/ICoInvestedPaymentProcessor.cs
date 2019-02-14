
using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface IPaymentProcessor
    {
        IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment);
    }

    public interface ICoInvestedPaymentProcessor : IPaymentProcessor
    {
    }


    public interface ILevyPaymentProcessor : IPaymentProcessor
    {
    }
}