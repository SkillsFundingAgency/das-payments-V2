using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public abstract class FundingSourcePaymentEvent : PeriodisedPaymentEvent, IFundingSourcePaymentEvent
    {
        public Guid RequiredPaymentEventId { get; set; }

        public ContractType ContractType { get; set; }

        public decimal SfaContributionPercentage { get; set; }

        public TransactionType TransactionType { get; set; }

        public FundingSourceType FundingSourceType { get; set; }
    }
}