using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.MessageInterceptors
{
    public abstract class InterceptJobMessages
    {
        private static readonly Dictionary<FundingSourceType, string> EventTypes;
        private static readonly string DefaultEventType = typeof(FundingSourcePaymentEvent).FullName;
        static InterceptJobMessages()
        {
            EventTypes = new Dictionary<FundingSourceType, string>
            {
                { FundingSourceType.CoInvestedEmployer, typeof(EmployerCoInvestedFundingSourcePaymentEvent).FullName },
                { FundingSourceType.CoInvestedSfa, typeof(SfaCoInvestedFundingSourcePaymentEvent).FullName },
                { FundingSourceType.FullyFundedSfa, typeof(SfaFullyFundedFundingSourcePaymentEvent).FullName },
                { FundingSourceType.Levy, typeof(LevyFundingSourcePaymentEvent).FullName },
                { FundingSourceType.Transfer, typeof(TransferFundingSourcePaymentEvent).FullName },
            };
        }

        protected static string GetFundingSourceMessageType(PaymentModel payment)
        {
            return EventTypes.ContainsKey(payment.FundingSource) ? EventTypes[payment.FundingSource] : DefaultEventType;
        }
    }
}