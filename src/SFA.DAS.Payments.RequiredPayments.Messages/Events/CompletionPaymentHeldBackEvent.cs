using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class CompletionPaymentHeldBackEvent : PeriodisedRequiredPaymentEvent
    {
        public TransactionType TransactionType { get; set; }
    }
}
