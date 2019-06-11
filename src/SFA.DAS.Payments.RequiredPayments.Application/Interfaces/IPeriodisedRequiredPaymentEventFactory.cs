using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    // ReSharper disable once IdentifierTypo
    public interface IPeriodisedRequiredPaymentEventFactory
    {
        PeriodisedRequiredPaymentEvent Create(EarningType earningType, int transactionType);
    }
}