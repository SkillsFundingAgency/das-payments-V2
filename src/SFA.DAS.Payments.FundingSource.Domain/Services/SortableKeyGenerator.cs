using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SortableKeyGenerator : IGenerateSortableKeys
    {
        public string Generate(ApprenticeshipContractType1RequiredPaymentEvent requiredPayment)
        {
            return string.Concat(requiredPayment.AmountDue < 0 ? "1" : "9", "-",
                requiredPayment.Priority.ToString("000000"), "-",
                requiredPayment.Learner.Uln);
        }
    }
}
