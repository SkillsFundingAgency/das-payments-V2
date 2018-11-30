using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public abstract class FundingSourcePaymentEvent : PaymentsEvent, IFundingSourcePaymentEvent
    {
        public Guid RequiredPaymentEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }

        public decimal AmountDue { get; set; }

        public CalendarPeriod DeliveryPeriod { get; set; }

        public ContractType ContractType { get; set; }

        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }

        public FundingSourceType FundingSourceType { get; set; }

    }
}