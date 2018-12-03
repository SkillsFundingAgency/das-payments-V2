using System;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPaymentFactory
    {
        ProviderPaymentEvent Create(FundingSourceType fundingSource);
    }

    public class ProviderPaymentFactory: IProviderPaymentFactory
    {
        public ProviderPaymentEvent Create(FundingSourceType fundingSource)
        {
            switch (fundingSource)
            {
                case FundingSourceType.CoInvestedEmployer:
                    return new EmployerCoInvestedProviderPaymentEvent();
                case FundingSourceType.CoInvestedSfa:
                    return new SfaCoInvestedProviderPaymentEvent();
                case FundingSourceType.FullyFundedSfa:
                    return new SfaFullyFundedProviderPaymentEvent();
                default:
                    throw new InvalidOperationException($"Cannot create the ProviderPayment, unexpected funding source: {fundingSource:G}");
            }
        }
    }
}