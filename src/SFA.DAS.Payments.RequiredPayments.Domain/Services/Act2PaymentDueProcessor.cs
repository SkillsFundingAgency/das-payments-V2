using System;
using System.Linq;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class Act2PaymentDueProcessor : IAct2PaymentDueProcessor
    {
        public ApprenticeshipContractType2RequiredPaymentEvent ProcessPaymentDue(ApprenticeshipContractType2PaymentDueEvent paymentDue, Payment[] paymentHistory)
        {
            if (paymentDue == null)
                throw new ArgumentNullException(nameof(paymentDue));

            if (paymentHistory == null)
                throw new ArgumentNullException(nameof(paymentHistory));

            // compare amounts
            var amountPaid = paymentHistory.Sum(p => p.Amount);

            if (amountPaid == paymentDue.AmountDue)
                return null;

            // create payment
            return new ApprenticeshipContractType2RequiredPaymentEvent
            {
                AmountDue = paymentDue.AmountDue - amountPaid,
                Learner = paymentDue.Learner.Clone(),
                Ukprn = paymentDue.Ukprn,
                CollectionPeriod = paymentDue.CollectionPeriod.Clone(),
                DeliveryPeriod = paymentDue.DeliveryPeriod.Clone(),
                LearningAim = paymentDue.LearningAim.Clone(),
                PriceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier,
                OnProgrammeEarningType = paymentDue.OnProgrammeEarningType,
                EventTime = DateTimeOffset.UtcNow,
                JobId = paymentDue.JobId
            };
        }
    }
}
