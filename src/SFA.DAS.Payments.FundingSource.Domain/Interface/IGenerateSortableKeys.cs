using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface IGenerateSortableKeys
    {
        string Generate(ApprenticeshipContractType1RequiredPaymentEvent requiredPayment);
    }
}
