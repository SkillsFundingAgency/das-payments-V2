using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.Tests.Core.Builders;
using Learner = SFA.DAS.Payments.Model.Core.Learner;
using Payment = SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Payment;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class CompletionPaymentHeldBackEventMatcher : BaseMatcher<CompletionPaymentHeldBackEvent>
    {
        private readonly Provider provider;
        private readonly CollectionPeriod collectionPeriod;
        private readonly List<Payment> paymentSpec;

        public CompletionPaymentHeldBackEventMatcher(
            Provider provider,
            CollectionPeriod collectionPeriod,
            List<Payment> paymentSpec)
        {
            this.provider = provider;
            this.collectionPeriod = collectionPeriod;
            this.paymentSpec = paymentSpec;
        }

        protected override IList<CompletionPaymentHeldBackEvent> GetActualEvents()
        {
            var events = RequiredPaymentEventHandler.ReceivedEvents
                .Where(e => e.Ukprn == provider.Ukprn &&
                            e.CollectionPeriod.Period == collectionPeriod.Period &&
                            e.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            e.JobId == provider.JobId).ToList();

            var results = new List<CompletionPaymentHeldBackEvent>();

            var heldBackEvents = events
                .Select(x => x as CompletionPaymentHeldBackEvent)
                .Where(x => x != null)
                .GroupBy(x => new
                {
                    x.DeliveryPeriod,
                    x.TransactionType,
                    x.Learner.ReferenceNumber,
                    x.LearningAim.Reference,
                    x.LearningAim.FrameworkCode,
                    x.LearningAim.StandardCode,
                });
            foreach (var aggregatedEvent in heldBackEvents)
            {
                results.Add(new CompletionPaymentHeldBackEvent
                {
                    AmountDue = aggregatedEvent.Sum(x => x.AmountDue),
                    DeliveryPeriod = aggregatedEvent.Key.DeliveryPeriod,
                    TransactionType = aggregatedEvent.Key.TransactionType,
                    LearningAim = new LearningAim
                    {
                        Reference = aggregatedEvent.Key.Reference,
                        FrameworkCode = aggregatedEvent.Key.FrameworkCode,
                        StandardCode = aggregatedEvent.Key.StandardCode,
                    },
                    Learner = new Learner { ReferenceNumber = aggregatedEvent.Key.ReferenceNumber },
                });
            }

            return results;
        }

        protected override IList<CompletionPaymentHeldBackEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<CompletionPaymentHeldBackEvent>();

            var paymentsToValidate =
                paymentSpec
                    .Where(e => e.ParsedCollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                                e.ParsedCollectionPeriod.Period == collectionPeriod.Period)
                    .ToList();

            foreach (var payment in paymentsToValidate)
            {
                foreach (var reason in payment.NonPaymentReasons.Keys)
                {
                    var amount = payment.NonPaymentReasons[reason];

                    if (amount != 0)
                        expectedPayments.Add(new CompletionPaymentHeldBackEvent
                        {
                            AmountDue = amount,
                            DeliveryPeriod = new DeliveryPeriodBuilder().WithSpecDate(payment.DeliveryPeriod).Build(),
                        });
                }
            }

            if(expectedPayments.Any())
                return expectedPayments;
            
            return expectedPayments;

        }

        protected override bool Match(CompletionPaymentHeldBackEvent expected, CompletionPaymentHeldBackEvent actual)
        {
            return expected.DeliveryPeriod == actual.DeliveryPeriod &&
                   expected.AmountDue == actual.AmountDue.AsRounded() &&
                   MatchNonPayment(expected as CompletionPaymentHeldBackEvent, actual as CompletionPaymentHeldBackEvent);
        }

        private bool MatchNonPayment(CompletionPaymentHeldBackEvent expected, CompletionPaymentHeldBackEvent actual)
        {
            if (expected == null)
                return true;

            return actual != null;
        }
    }
}