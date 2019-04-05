using System;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    [KnownType("GetInheritors")]
    public abstract class ProviderPaymentEvent : PeriodisedPaymentEvent, IProviderPaymentEvent
    {
        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(ProviderPaymentEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(ProviderPaymentEvent)))
                       .ToArray());
        }

        public TransactionType TransactionType { get; set; }
        public FundingSourceType FundingSourceType { get; set; }  //Backwards Compatibility
        public decimal SfaContributionPercentage { get; set; }
    }
}
