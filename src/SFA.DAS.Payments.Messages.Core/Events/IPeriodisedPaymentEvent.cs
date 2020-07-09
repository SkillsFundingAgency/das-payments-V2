using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IPeriodisedPaymentEvent : IPaymentsEvent
    {
        Guid EarningEventId { get; }
        string PriceEpisodeIdentifier { get; }
        decimal AmountDue { get; }
        byte DeliveryPeriod { get; }
        long? AccountId { get; }
        long? TransferSenderAccountId { get; set; }
        ContractType ContractType { get; }
        DateTime StartDate { get; }
        DateTime? PlannedEndDate { get;}
        DateTime? ActualEndDate { get; }
        byte CompletionStatus { get; }
        decimal CompletionAmount { get; }
        decimal InstalmentAmount { get; }
        short NumberOfInstalments { get; }
        DateTime? LearningStartDate { get; }
        long? ApprenticeshipId { get; set; }
        long? ApprenticeshipPriceEpisodeId { get; set; }
        ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        TransactionType TransactionType { get; }
    }
}