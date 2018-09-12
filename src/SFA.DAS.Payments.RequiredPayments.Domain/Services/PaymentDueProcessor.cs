using System;
using System.Linq;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class PaymentDueProcessor : IPaymentDueProcessor
    {
        public RequiredPaymentEvent ProcessPaymentDue(PaymentDueEvent paymentDue, Payment[] paymentHistory)
        {
            if (paymentDue == null)
                throw new ArgumentNullException(nameof(paymentDue));

            if (paymentHistory == null)
                throw new ArgumentNullException(nameof(paymentHistory));

            // compare amounts
            var amountPaid = paymentHistory.Sum(p => p.Amount);

            if (amountPaid == paymentDue.Amount)
                return null;

            // create payment
            return new RequiredPaymentEvent
            {
                Amount = paymentDue.Amount - amountPaid,
                Learner = paymentDue.Learner.Clone(),
                Ukprn = paymentDue.Ukprn,
                CollectionPeriod = paymentDue.CollectionPeriod.Clone(),
                DeliveryPeriod = paymentDue.DeliveryPeriod.Clone(),
                LearningAim = paymentDue.LearningAim.Clone(),
                PriceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier,
                EventTime = DateTimeOffset.UtcNow,
                JobId = paymentDue.JobId
            };
        }
    }
}
