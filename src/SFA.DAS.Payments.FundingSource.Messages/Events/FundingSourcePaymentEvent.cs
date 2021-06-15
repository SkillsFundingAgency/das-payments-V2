using System;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class FundingSourcePaymentEvent : PeriodisedPaymentEvent, IFundingSourcePaymentEvent, IMonitoredMessage
    {
        public Guid RequiredPaymentEventId { get; set; }

        public decimal SfaContributionPercentage { get; set; }

         public new TransactionType TransactionType { get; set; }

        public FundingSourceType FundingSourceType { get; set; }

        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(FundingSourcePaymentEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(FundingSourcePaymentEvent)))
                       .ToArray());
        }
    }
}