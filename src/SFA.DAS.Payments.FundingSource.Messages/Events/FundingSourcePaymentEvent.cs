using SFA.DAS.Payments.FundingSource.Model.Enum;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using System;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public abstract class FundingSourcePaymentEvent : PaymentsEvent, IFundingSourcePaymentEvent
    {
        public string PriceEpisodeIdentifier { get; set; }

        public decimal AmountDue { get; set; }

        public CalendarPeriod DeliveryPeriod { get; set; }
        
        public byte ContractType { get; set; }

        public decimal SfaContributionPercentage { get; set; }

        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }

        public FundingSourceType FundingSourceType { get; set; }

    }
}