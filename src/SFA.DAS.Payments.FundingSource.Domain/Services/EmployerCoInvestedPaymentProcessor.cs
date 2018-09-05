using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class EmployerCoInvestedPaymentProcessor : CoInvestedPaymentProcessor, ICoInvestedPaymentProcessor
    {
        public FundingSourcePaymentEvent Process(ApprenticeshipContractType2RequiredPaymentEvent message)
        {
            ValidateSfaContributionPercentage(message);

            var amountToPay = (1 - message.SfaContributionPercentage) * message.AmountDue;

            return new FundingSourcePaymentEvent
            {
                JobId = message.JobId,
                EventTime = DateTime.UtcNow,
                Amount = amountToPay
            };
        }
    }
}