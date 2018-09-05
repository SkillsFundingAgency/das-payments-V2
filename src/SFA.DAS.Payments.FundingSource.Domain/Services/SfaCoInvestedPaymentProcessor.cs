using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaCoInvestedPaymentProcessor : CoInvestedPaymentProcessor , ICoInvestedPaymentProcessor
    {
        public FundingSourcePaymentEvent Process(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            ValidateSfaContributionPercentage(message);
          
            var amountToPay = message.SfaContributionPercentage * message.AmountDue;

            return new FundingSourcePaymentEvent
            {
                JobId = message.JobId,
                EventTime = DateTime.UtcNow,
                Amount = amountToPay
            };
        }
    }
}