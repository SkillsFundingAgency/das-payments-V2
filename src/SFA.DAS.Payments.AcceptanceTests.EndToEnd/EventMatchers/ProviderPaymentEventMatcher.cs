using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers
{
    public class ProviderPaymentEventMatcher : BaseMatcher<ProviderPaymentEvent>
    {
        private readonly List<ProviderPayment> paymentSpec;
        private readonly TestSession testSession;
        private readonly CalendarPeriod collectionPeriod;

        public ProviderPaymentEventMatcher(CalendarPeriod collectionPeriod, TestSession testSession, List<ProviderPayment> paymentSpec)
        {
            this.collectionPeriod = collectionPeriod;
            this.testSession = testSession;
            this.paymentSpec = paymentSpec;
        }

        protected override IList<ProviderPaymentEvent> GetActualEvents()
        {
            return ProviderPaymentEventHandler.ReceivedEvents
                .Where(ev =>
                    ev.Ukprn == testSession.Ukprn &&
                    ev.JobId == testSession.JobId)
                .ToList();

        }

        protected override IList<ProviderPaymentEvent> GetExpectedEvents()
        {
            var expectedPayments = new List<ProviderPaymentEvent>();

            foreach (var providerPayment in paymentSpec.Where(p => p.CollectionPeriod.ToDate().ToCalendarPeriod().Name == collectionPeriod.Name))
            {
                var eventCollectionPeriod = providerPayment.CollectionPeriod.ToCalendarPeriod();
                var deliveryPeriod = providerPayment.DeliveryPeriod.ToCalendarPeriod();
                var learner = new Learner { ReferenceNumber = testSession.GenerateLearnerReference(providerPayment.LearnerId)};

                var coFundedSfa = new SfaCoInvestedProviderPaymentEvent
                {
                    TransactionType = providerPayment.TransactionType,
                    AmountDue = providerPayment.SfaCoFundedPayments,
                    CollectionPeriod = eventCollectionPeriod,
                    DeliveryPeriod = deliveryPeriod,
                    Learner = learner
                };

                var coFundedEmp = new EmployerCoInvestedProviderPaymentEvent
                {
                    TransactionType = providerPayment.TransactionType,
                    AmountDue = providerPayment.EmployerCoFundedPayments,
                    CollectionPeriod = eventCollectionPeriod,
                    DeliveryPeriod = deliveryPeriod,
                    Learner = learner
                };

                expectedPayments.Add(coFundedSfa);
                expectedPayments.Add(coFundedEmp);
            }

            return expectedPayments;
        }

        protected override bool Match(ProviderPaymentEvent expected, ProviderPaymentEvent actual)
        {
            return expected.GetType() == actual.GetType() &&
                   expected.TransactionType == actual.TransactionType &&
                   expected.AmountDue == actual.AmountDue &&
                   expected.CollectionPeriod.Name == actual.CollectionPeriod.Name &&
                   expected.DeliveryPeriod.Name == actual.DeliveryPeriod.Name &&
                   expected.Learner.ReferenceNumber == actual.Learner.ReferenceNumber;
        }
    }
}