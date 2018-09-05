using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public abstract class CoInvestedPaymentProcessor
    {
        protected void ValidateSfaContributionPercentage(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            if (message.SfaContributionPercentage == 0 )
                throw new ArgumentException("Sfa Contribution Percentage cannot be zero");
        }
    }
}