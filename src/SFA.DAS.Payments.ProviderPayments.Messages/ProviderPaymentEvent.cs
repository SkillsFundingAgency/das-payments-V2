using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    public class EmployerCoInvestedProviderPaymentEvent: CoInvestedProviderPaymentEvent
    {

    }

    public class SfaCoInvestedProviderPaymentEvent: CoInvestedProviderPaymentEvent
    {

    }

    public abstract class CoInvestedProviderPaymentEvent: ProviderPaymentEvent
    {

    }

    public abstract class ProviderPaymentEvent: IPeriodisedPaymentEvent
    {
        public Guid ExternalId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }
        public Learner Learner { get; set; }
        public LearningAim LearningAim { get; set; }
        public long JobId { get; set; }
        public byte ContractType { get; set; } //Backwards Compatibility //TODO: we will eventually have events: ACT1Transfer, ACT1Levy, ACT1EmployerCoInvested, ACT1SfaCoInvested, ACT2EmployerCoInvested, etc
        public TransactionType TransactionType { get; set; }
        public FundingSourceType FundingSourceType { get; set; }  //Backwards Compatibility
        public DateTime IlrSubmissionDateTime { get; set; }
        public decimal SfaContributionPercentage { get; set; }
    }
}
