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

            // TODO: validate paymentDue
            
            // compare amounts
            var amountPaid = paymentHistory.Sum(p => p.Amount);

            // create payment
            if (amountPaid == paymentDue.Amount)
                return null;

            return new RequiredPaymentEvent
            {
                Amount = paymentDue.Amount - amountPaid,
                Learner = new Learner
                {
                    Ukprn = paymentDue.Ukprn,
                    ReferenceNumber = paymentDue.Learner.ReferenceNumber,
                    Uln = paymentDue.Learner.Uln
                },
                Ukprn = paymentDue.Ukprn,
                CollectionPeriod = paymentDue.CollectionPeriod,
                DeliveryPeriod = paymentDue.DeliveryPeriod,
                LearningAim = paymentDue.LearningAim,
                PriceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier,
                EventTime = DateTimeOffset.UtcNow,
                JobId = paymentDue.JobId
            };
        }
    }
}
