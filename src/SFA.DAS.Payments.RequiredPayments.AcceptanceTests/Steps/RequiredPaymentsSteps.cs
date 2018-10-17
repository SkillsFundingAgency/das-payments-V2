using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsSteps : RequiredPaymentsStepsBase
    {
        private static bool trace = true;

        public RequiredPaymentsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When(@"a payments due event is received")]
        public async Task WhenAPaymentsDueEventIsReceived()
        {
            var payments = PaymentsDue.Select(CreatePaymentDueEvent).ToList();
            foreach (var paymentDue in payments)
            {
                await MessageSession.Send(paymentDue).ConfigureAwait(false);
            }
        }

        private ApprenticeshipContractTypePaymentDueEvent CreatePaymentDueEvent(OnProgrammePaymentDue paymentDue)
        {
            var payment = ContractType == 1
                ? (ApprenticeshipContractTypePaymentDueEvent)new ApprenticeshipContractType1PaymentDueEvent()
                : new ApprenticeshipContractType2PaymentDueEvent();

            payment.Learner = TestSession.Learner.ToLearner();
            payment.Learner.ReferenceNumber = TestSession.GenerateLearnerReference(paymentDue.LearnerId);
            payment.Ukprn = TestSession.Ukprn;
            payment.SfaContributionPercentage = SfaContributionPercentage;
            payment.Type = paymentDue.Type;
            payment.AmountDue = paymentDue.Amount;
            payment.CollectionPeriod = new CalendarPeriod(CollectionYear, CollectionPeriod);
            payment.DeliveryPeriod = new CalendarPeriod(CollectionYear, paymentDue.DeliveryPeriod);
            payment.JobId = TestSession.JobId;
            payment.LearningAim = TestSession.Learner.Course.ToLearningAim();
            payment.PriceEpisodeIdentifier = paymentDue.PriceEpisodeIdentifier;
            return payment;
        }

        [Then(@"the required payments component will generate the following contract type (.*) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypePayableEarnings(int p0, Table table)
        {
            var expectedPaymentsEvents = table
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents), "Failed to find all the required payment events");
        }

        [Then(@"the required payments component will generate the following contract type (.*) Completion \(TT(.*)\) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypeCompletionPayableEarnings(byte contractType, int transactionType, Table expectedEventsTable)
        {
            var expectedPaymentsEvents = expectedEventsTable
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents, OnProgrammeEarningType.Completion), "Failed to find all the required payment events");
        }

        [Then(@"the required payments component will generate the following contract type (.*) Learning \(TT(.*)\) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypeLearningPayableEarnings(byte contractType, int transactionType, Table expectedEventsTable)
        {
            var expectedPaymentsEvents = expectedEventsTable
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents, OnProgrammeEarningType.Learning), "Failed to find all the required payment events");
        }

        [Then(@"the required payments component will generate the following contract type (.*) Balancing \(TT(.*)\) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypeBalancingPayableEarnings(byte contractType, int transactionType, Table expectedEventsTable)
        {
            var expectedPaymentsEvents = expectedEventsTable
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents, OnProgrammeEarningType.Balancing), "Failed to find all the required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Learning \(TT(.*)\) payable earnings")]
        public void ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeLearningTTPayableEarnings(int p0, int p1)
        {
            WaitForIt(() => MatchUnexpectedRequiredPayment(OnProgrammeEarningType.Learning), "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Completion \(TT(.*)\) payable earnings")]
        public void ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeCompletionTTPayableEarnings(int p0, int p1)
        {
            WaitForIt(() => MatchUnexpectedRequiredPayment(OnProgrammeEarningType.Completion), "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Balancing \(TT(.*)\) payable earnings")]
        public void ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeBalancingTTPayableEarnings(int p0, int p1)
        {
            WaitForIt(() => MatchUnexpectedRequiredPayment(OnProgrammeEarningType.Balancing), "Found some unexpected required payment events");
        }

        private bool MatchUnexpectedRequiredPayment(OnProgrammeEarningType? type)
        {
            var result = PaymentsDue.Where(x => !type.HasValue || x.Type == type).ToList().All(paymentDue =>
                !ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent =>
                    paymentDue.Amount == receivedEvent.AmountDue
                    && TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber
                    && paymentDue.Type == receivedEvent.OnProgrammeEarningType
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && paymentDue.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                ));
#if DEBUG
            if (!result && trace)
            {
                Debug.WriteLine("Found unexpected events. Trace:");
                TraceMatch(PaymentsDue.ToArray(), ApprenticeshipContractType2Handler.ReceivedEvents.ToArray());
            }
#endif
            return result;
        }

        private bool MatchRequiredPayment(OnProgrammePaymentDue[] expectedPaymentsEvents, OnProgrammeEarningType? type = null)
        {
            OnProgrammePaymentDue[] events;

            if (type.HasValue)
            {
                events = expectedPaymentsEvents.Where(x => x.Type == type.Value).ToArray();

                if (events.Length == 0)
                    throw new ArgumentException("expectedPayments have no items of requested type");
            }
            else
            {
                events = expectedPaymentsEvents;
            }

            var sessionEvents = ApprenticeshipContractType2Handler.ReceivedEvents.Where(e => e.Ukprn == TestSession.Ukprn).ToArray();

            var matchedExpectations = sessionEvents
                .Where(receivedEvent => events.Any(expectedEvent =>
                    expectedEvent.Amount == receivedEvent.AmountDue
                    && TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber
                    && expectedEvent.Type == receivedEvent.OnProgrammeEarningType
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                )).ToList();

            var allFound = matchedExpectations.Count == expectedPaymentsEvents.Length;
            var nothingExtra = sessionEvents.Length == matchedExpectations.Count;

#if DEBUG
            if ((!allFound || !nothingExtra) && trace)
            {
                if (!allFound)
                    Debug.WriteLine("Did not find all expected events. Trace:");
                else
                    Debug.WriteLine("Found some unexpected events. Trace:");

                TraceMatch(expectedPaymentsEvents, sessionEvents);
            }
#endif
            return allFound && nothingExtra;
        }

        private void TraceMatch(OnProgrammePaymentDue[] expectedPaymentsEvents, ApprenticeshipContractType2RequiredPaymentEvent[] receivedEvents)
        {
            for (var i = 0; i < expectedPaymentsEvents.Length; i++)
            {
                var expectedEvent = expectedPaymentsEvents[i];
                for (var k = 0; k < receivedEvents.Length; k++)
                {
                    var receivedEvent = receivedEvents[k];
                    var mismatchedFields = new List<string>();

                    if (expectedEvent.Amount != receivedEvent.AmountDue) mismatchedFields.Add($" Amount({expectedEvent.Amount}!={receivedEvent.AmountDue})");
                    if (TestSession.Learner.LearnRefNumber != receivedEvent?.Learner?.ReferenceNumber) mismatchedFields.Add($"LearnRefNumber({TestSession.Learner.LearnRefNumber}!={receivedEvent?.Learner?.ReferenceNumber})");
                    if (expectedEvent.Type != receivedEvent.OnProgrammeEarningType) mismatchedFields.Add($"Type({expectedEvent.Type}!={receivedEvent.OnProgrammeEarningType})");
                    if (TestSession.Ukprn != receivedEvent.Ukprn) mismatchedFields.Add($"Ukprn({TestSession.Ukprn}!={receivedEvent.Ukprn})");
                    if (expectedEvent.DeliveryPeriod != receivedEvent.DeliveryPeriod?.Period) mismatchedFields.Add($"Period({expectedEvent.DeliveryPeriod}!={receivedEvent.DeliveryPeriod?.Period})");
                    if (!receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)) mismatchedFields.Add($"CollectionPeriod({receivedEvent.CollectionPeriod} does not contain {CollectionYear})");

                    if (mismatchedFields.Count == 0)
                        Debug.WriteLine($"Event {i + 1} of {expectedPaymentsEvents.Length}: match {k + 1}");
                    else
                        Debug.WriteLine($"Event {i + 1} of {expectedPaymentsEvents.Length}: mismatch {k + 1} on {string.Join(",", mismatchedFields)}");
                }
            }
        }
    }
}