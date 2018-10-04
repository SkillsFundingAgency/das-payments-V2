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
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsSteps : RequiredPaymentsStepsBase
    {
        private static bool trace = false;

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

        private ApprenticeshipContractTypePaymentDueEvent CreatePaymentDueEvent(Data.OnProgrammePaymentDue paymentDue)
        {
            var payment = ContractType == 1
                ? (ApprenticeshipContractTypePaymentDueEvent)new ApprenticeshipContractType1PaymentDueEvent()
                : new ApprenticeshipContractType2PaymentDueEvent();

            payment.Learner = TestSession.Learner.ToLearner();
            payment.Ukprn = TestSession.Ukprn;
            payment.SfaContributionPercentage = SfaContributionPercentage;
            payment.Type = paymentDue.Type;
            payment.AmountDue = paymentDue.Amount;
            payment.CollectionPeriod = new CalendarPeriod(CollectionYear, CollectionPeriod);
            payment.DeliveryPeriod = new CalendarPeriod(CollectionYear, paymentDue.Period);
            payment.JobId = TestSession.JobId;
            payment.LearningAim = TestSession.Learner.Course.ToLearningAim();
            payment.PriceEpisodeIdentifier = "p-1"; //TODO: will need to change if scenario specifies different identifier
            return payment;
        }

        [Then(@"the required payments component will generate the following contract type (.*) Completion \(TT(.*)\) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypeCompletionPayableEarnings(byte contractType, int transactionType, Table expectedEventsTable)
        {
            var expectedPaymentsEvents = expectedEventsTable
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents), "Failed to find all the required payment events");
        }

        private bool MatchRequiredPayment(OnProgrammePaymentDue[] expectedPaymentsEvents)
        {
            var result = expectedPaymentsEvents.Where(x => x.Type == OnProgrammeEarningType.Completion).All(expectedEvent =>
                ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent =>
                    expectedEvent.Amount == receivedEvent.AmountDue
                    && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                    && expectedEvent.Type == receivedEvent.OnProgrammeEarningType
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && expectedEvent.Period == receivedEvent.DeliveryPeriod?.Period
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                ));
#if DEBUG
            if (!result && trace)
            {
                Debug.WriteLine("Did not find all expected events. Trace:");
                for (var i = 0; i < expectedPaymentsEvents.Length; i++)
                {
                    var expectedEvent = expectedPaymentsEvents[i];
                    foreach (var receivedEvent in ApprenticeshipContractType2Handler.ReceivedEvents)
                    {
                        var mismatchedFields = new List<string>();

                        if (expectedEvent.Amount != receivedEvent.AmountDue) mismatchedFields.Add($" Amount({expectedEvent.Amount}!={receivedEvent.AmountDue})");
                        if (TestSession.Learner.LearnRefNumber != receivedEvent?.Learner?.ReferenceNumber) mismatchedFields.Add($"LearnRefNumber({TestSession.Learner.LearnRefNumber}!={receivedEvent?.Learner?.ReferenceNumber})");
                        if (expectedEvent.Type != receivedEvent.OnProgrammeEarningType) mismatchedFields.Add($"Type({expectedEvent.Type}!={receivedEvent.OnProgrammeEarningType})");
                        if (TestSession.Ukprn != receivedEvent.Ukprn) mismatchedFields.Add($"Ukprn({TestSession.Ukprn}!={receivedEvent.Ukprn})");
                        if (expectedEvent.Period != receivedEvent.DeliveryPeriod?.Period) mismatchedFields.Add($"Period({expectedEvent.Period}!={receivedEvent.DeliveryPeriod?.Period})");
                        if (!receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)) mismatchedFields.Add($"CollectionPeriod({receivedEvent.CollectionPeriod} does not contain {CollectionYear})");

                        if (mismatchedFields.Count == 0)
                            Debug.WriteLine($"Event {i} of {expectedPaymentsEvents.Length}: match");
                        else
                            Debug.WriteLine($"Event {i} of {expectedPaymentsEvents.Length}: mismatch on {string.Join(",", mismatchedFields)}");
                    }
                }
            }
#endif
            return result;
        }

        [Then(@"the required payments component will generate the following contract type (.*) Learning \(TT(.*)\) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypeLearningPayableEarnings(byte contractType, int transactionType, Table expectedEventsTable)
        {
            var expectedPaymentsEvents = expectedEventsTable
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents), "Failed to find all the required payment events");
        }

        [Then(@"the required payments component will generate the following contract type (.*) Balancing \(TT(.*)\) payable earnings:")]
        public void ThenTheRequiredPaymentsComponentWillGenerateTheFollowingContractTypeBalancingPayableEarnings(byte contractType, int transactionType, Table expectedEventsTable)
        {
            var expectedPaymentsEvents = expectedEventsTable
                .CreateSet<OnProgrammePaymentDue>().ToArray();//TODO: fix to use a required payments model
            WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents), "Failed to find all the required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Learning \(TT(.*)\) payable earnings")]
        public void ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeLearningTTPayableEarnings(int p0, int p1)
        {
            WaitForIt(() =>
            {
                return PaymentsDue.Where(x => x.Type == OnProgrammeEarningType.Learning).All(paymentDue =>
                    !ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent =>
                        paymentDue.Amount == receivedEvent.AmountDue
                        && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                        && paymentDue.Type == receivedEvent.OnProgrammeEarningType
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && paymentDue.Period == receivedEvent.DeliveryPeriod?.Period
                        && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                    ));
            }, "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Completion \(TT(.*)\) payable earnings")]
        public void ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeCompletionTTPayableEarnings(int p0, int p1)
        {
            WaitForIt(() =>
            {
                return PaymentsDue.Where(x => x.Type == OnProgrammeEarningType.Completion).All(paymentDue =>
                    !ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent =>
                            paymentDue.Amount == receivedEvent.AmountDue
                            && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                            && paymentDue.Type == receivedEvent.OnProgrammeEarningType
                            && TestSession.Ukprn == receivedEvent.Ukprn
                            && paymentDue.Period == receivedEvent.DeliveryPeriod?.Period
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                    ));
            }, "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Balancing \(TT(.*)\) payable earnings")]
        public void ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeBalancingTTPayableEarnings(int p0, int p1)
        {
            WaitForIt(() =>
            {
                return PaymentsDue.Where(x => x.Type == OnProgrammeEarningType.Balancing).All(paymentDue =>
                    !ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent =>
                            paymentDue.Amount == receivedEvent.AmountDue
                            && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                            && paymentDue.Type == receivedEvent.OnProgrammeEarningType
                            && TestSession.Ukprn == receivedEvent.Ukprn
                            && paymentDue.Period == receivedEvent.DeliveryPeriod?.Period
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                    ));
            }, "Found some unexpected required payment events");
        }
    }
}