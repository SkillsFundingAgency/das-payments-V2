using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Handlers;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using OnProgrammeEarning = SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data.OnProgrammeEarning;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class RequiredPaymentsSteps : RequiredPaymentsStepsBase
    {
        public RequiredPaymentsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When(@"an earning event is received")]
        public async Task WhenAnEarningEventIsReceived()
        {
            var earnings = Earnings.GroupBy(p => p.LearnerId).Select(CreateEarningEvent).ToList();

            foreach (var earningEvent in earnings)
            {
                await MessageSession.Send(earningEvent).ConfigureAwait(false);
            }
        }

        private ApprenticeshipContractTypeEarningsEvent CreateEarningEvent(IGrouping<string, OnProgrammeEarning> group)
        {

            var earningEvent = ContractType == 1
                ? (ApprenticeshipContractTypeEarningsEvent)new ApprenticeshipContractType1EarningEvent()
                : new ApprenticeshipContractType2EarningEvent();

            var testSessionLearner =
                TestSession.Learners.FirstOrDefault(l => l.LearnerIdentifier == group.Key) ??
                TestSession.Learner;

            earningEvent.Learner = testSessionLearner.ToLearner();
            earningEvent.Ukprn = TestSession.Ukprn;
            earningEvent.SfaContributionPercentage = SfaContributionPercentage;
            earningEvent.CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(CollectionYear, CollectionPeriod);
            earningEvent.CollectionYear = CollectionYear;
            earningEvent.JobId = TestSession.JobId;
            earningEvent.LearningAim = testSessionLearner.Course.ToLearningAim();

            var onProgrammeEarnings = new List<Payments.Model.Core.OnProgramme.OnProgrammeEarning>();

            foreach (var learnerEarning in group)
            {
                var onProgrammeEarning = new Payments.Model.Core.OnProgramme.OnProgrammeEarning
                {
                    Type = learnerEarning.Type,
                    Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                    {
                        new EarningPeriod
                        {
                            Amount = learnerEarning.Amount,
                            Period = learnerEarning.DeliveryPeriod,
                            PriceEpisodeIdentifier = learnerEarning.PriceEpisodeIdentifier
                        }

                    })
                };
                onProgrammeEarnings.Add(onProgrammeEarning);
            }

            earningEvent.OnProgrammeEarnings = new ReadOnlyCollection<Payments.Model.Core.OnProgramme.OnProgrammeEarning>(onProgrammeEarnings);
            return earningEvent;
        }

        [Then(@"the required payments component will only generate contract type (.*) required payments")]
        public async Task ThenTheRequiredPaymentsComponentWillOnlyGenerateContractTypeRequiredPayments(int p0, Table table)
        {
            var expectedPaymentsEvents = table
                .CreateSet<OnProgrammeEarning>().ToArray();//TODO: fix to use a required payments model
            await WaitForIt(() => MatchRequiredPayment(expectedPaymentsEvents), "Failed to find all the required payment events or unexpected events found");
        }

        [Then(@"the required payments component will not generate any contract type (.*) payable earnings")]
        public async Task ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypePayableEarnings(int p0)
        {
            await WaitForIt(() => MatchUnexpectedRequiredPayment(null), "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Learning \(TT(.*)\) payable earnings")]
        public async Task ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeLearningTTPayableEarnings(int p0, int p1)
        {
            await WaitForIt(() => MatchUnexpectedRequiredPayment(OnProgrammeEarningType.Learning), "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Completion \(TT(.*)\) payable earnings")]
        public async Task ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeCompletionTTPayableEarnings(int p0, int p1)
        {
            await WaitForIt(() => MatchUnexpectedRequiredPayment(OnProgrammeEarningType.Completion), "Found some unexpected required payment events");
        }

        [Then(@"the required payments component will not generate any contract type (.*) Balancing \(TT(.*)\) payable earnings")]
        public async Task ThenTheRequiredPaymentsComponentWillNotGenerateAnyContractTypeBalancingTTPayableEarnings(int p0, int p1)
        {
            await WaitForIt(() => MatchUnexpectedRequiredPayment(OnProgrammeEarningType.Balancing), "Found some unexpected required payment events");
        }

        private bool MatchUnexpectedRequiredPayment(OnProgrammeEarningType? type)
        {
            var result = Earnings.Where(x => !type.HasValue || x.Type == type).ToList().All(earning =>
                !ApprenticeshipContractType2Handler.ReceivedEvents.Any(receivedEvent =>
                    TestSession.JobId == receivedEvent.JobId
                    && earning.Amount == receivedEvent.AmountDue
                    && TestSession.Learners.Any(l => l.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber)
                    && earning.Type == receivedEvent.OnProgrammeEarningType
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && earning.DeliveryPeriod == receivedEvent.DeliveryPeriod
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear.ToString())
                ));
#if DEBUG
            if (!result)
            {
                Debug.WriteLine("Found unexpected events. Trace:");
                TraceMismatch(Earnings.ToArray(), ApprenticeshipContractType2Handler.ReceivedEvents.ToArray());
            }
#endif
            return result;
        }

        private (bool pass, string reason)MatchRequiredPayment(OnProgrammeEarning[] expectedPaymentsEvents, OnProgrammeEarningType? type = null)
        {
            OnProgrammeEarning[] events;

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
                    && TestSession.JobId == receivedEvent.JobId
                    && TestSession.Learners.FirstOrDefault(l => l.LearnerIdentifier == expectedEvent.LearnerId)?.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber  //TestSession.Learners.Any(l => l.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber)  
                    && expectedEvent.Type == receivedEvent.OnProgrammeEarningType
                    && TestSession.Ukprn == receivedEvent.Ukprn
                    && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod
                    && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear.ToString())
                )).ToList();

            var allFound = matchedExpectations.Count == expectedPaymentsEvents.Length;
            var nothingExtra = sessionEvents.Length == matchedExpectations.Count;

            var reason = new List<string>();
            if (!allFound)
                reason.Add($"{expectedPaymentsEvents.Length - matchedExpectations.Count} out of {expectedPaymentsEvents.Length} event(s) did not arrive");

            if (!nothingExtra)
                reason.Add($"{sessionEvents.Length - matchedExpectations.Count} unexpected event(s) arrived");

#if DEBUG
            if ((!allFound || !nothingExtra))
            {
                if (!allFound)
                {
                    Debug.WriteLine("Did not find all expected events. Trace:");
                    TraceMismatch(expectedPaymentsEvents, sessionEvents);
                }

                if (!nothingExtra)
                {
                    Debug.WriteLine("Found some unexpected events. Trace:");
                    TraceUnexpected(sessionEvents, matchedExpectations);
                }
            }
#endif
            return (allFound && nothingExtra, string.Join(" and ", reason));
        }

        private static void TraceUnexpected(ApprenticeshipContractType2RequiredPaymentEvent[] sessionEvents, List<ApprenticeshipContractType2RequiredPaymentEvent> matchedExpectations)
        {
            var unexpected = sessionEvents.Where(e => !matchedExpectations.Contains(e)).ToList();
            for (var i = 0; i < unexpected.Count; i++)
            {
                var e = unexpected[i];
                Debug.WriteLine($"{i + 1}: PE:{e.PriceEpisodeIdentifier}, AmountDue:{e.AmountDue}, " +
                                $"LearnRefNumber:{e.Learner.ReferenceNumber}, Type:{e.OnProgrammeEarningType}, " +
                                $"DeliveryPeriod:{e.DeliveryPeriod}, CollectionPeriod:{e.CollectionPeriod.Name}");
            }
        }

        private void TraceMismatch(OnProgrammeEarning[] expectedPaymentsEvents, ApprenticeshipContractType2RequiredPaymentEvent[] receivedEvents)
        {
            for (var i = 0; i < expectedPaymentsEvents.Length; i++)
            {
                var expectedEvent = expectedPaymentsEvents[i];
                for (var k = 0; k < receivedEvents.Length; k++)
                {
                    var receivedEvent = receivedEvents[k];
                    var mismatchedFields = new List<string>();
                    var learnerReference = TestSession.GenerateLearnerReference(expectedEvent.LearnerId);

                    if (expectedEvent.Amount != receivedEvent.AmountDue) mismatchedFields.Add($" Amount({expectedEvent.Amount}!={receivedEvent.AmountDue})");
                    if (learnerReference != receivedEvent.Learner?.ReferenceNumber) mismatchedFields.Add($"LearnRefNumber({learnerReference}!={receivedEvent.Learner?.ReferenceNumber})");
                    if (expectedEvent.Type != receivedEvent.OnProgrammeEarningType) mismatchedFields.Add($"Type({expectedEvent.Type}!={receivedEvent.OnProgrammeEarningType})");
                    if (expectedEvent.DeliveryPeriod != receivedEvent.DeliveryPeriod) mismatchedFields.Add($"Period({expectedEvent.DeliveryPeriod}!={receivedEvent.DeliveryPeriod})");
                    if (!receivedEvent.CollectionPeriod.Name.Contains(CollectionYear.ToString())) mismatchedFields.Add($"CollectionPeriod({receivedEvent.CollectionPeriod} does not contain {CollectionYear})");

                    if (mismatchedFields.Count == 0)
                        Debug.WriteLine($"Event {i + 1} of {expectedPaymentsEvents.Length}: match {k + 1}");
                    else
                        Debug.WriteLine($"Event {i + 1} of {expectedPaymentsEvents.Length}: mismatch {k + 1} on {string.Join(",", mismatchedFields)}");
                }
            }
        }
    }
}