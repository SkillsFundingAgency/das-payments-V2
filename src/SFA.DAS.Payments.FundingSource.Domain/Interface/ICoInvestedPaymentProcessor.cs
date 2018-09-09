
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ICoInvestedPaymentProcessor
    {
        Payment Process(CoInvestedPayment message);
    }
}