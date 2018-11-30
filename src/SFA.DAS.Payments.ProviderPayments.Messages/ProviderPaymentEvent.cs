using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;

namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    public abstract class ProviderPaymentEvent : PaymentsEvent, IPeriodisedPaymentEvent
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        public byte ContractType { get; set; } //Backwards Compatibility //TODO: we will eventually have events: ACT1Transfer, ACT1Levy, ACT1EmployerCoInvested, ACT1SfaCoInvested, ACT2EmployerCoInvested, etc
        public TransactionType TransactionType { get; set; }
        public FundingSourceType FundingSourceType { get; set; }  //Backwards Compatibility
        public decimal SfaContributionPercentage { get; set; }
    }
}
