using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ILevyPaymentProcessor
    {
        FundingSourcePayment Process(RequiredLevyPayment requiredPayment, ref decimal levyBalance);
    }
}