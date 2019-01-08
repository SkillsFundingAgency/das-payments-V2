using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Handlers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using OnProgrammeEarning = SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data.OnProgrammeEarning;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueSteps : StepsBase
    {
        private const bool trace = true;

        public PaymentsDueSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When(@"an earnings event is received")]
        public async Task WhenAnEarningsEventIsReceived()
        {
            foreach (var act2EarningEvent in Act2EarningEvents)
            {
                act2EarningEvent.CollectionPeriod = Model.Core.CollectionPeriod.CreateFromAcademicYearAndPeriod(CollectionYear, CollectionPeriod);
                await MessageSession.Send(act2EarningEvent);
            }
        }

        [Then(@"the payments due component will generate the following contract type (.*) payments due:")]
        public async Task ThenThePaymentsDueComponentWillGenerateTheFollowingContractTypePaymentsDue(int act, Table table)
        {
            var expectedPaymentsEvents = table.CreateSet<OnProgrammePaymentDue>().ToList();
            await WaitForIt(() => MatchPaymentDue(expectedPaymentsEvents), "Failed to find all the payment due events");
        }

        private bool MatchPaymentDue(IReadOnlyCollection<OnProgrammePaymentDue> expectedPaymentsEvents)
        {
            var sessionEvents = ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Where(r => r.Ukprn == TestSession.Ukprn).ToList();

            var matchedReceivedEvents = sessionEvents.Where(receivedEvent =>
            {
                return expectedPaymentsEvents.Any(expectedEvent =>
                {
                    return expectedEvent.PriceEpisodeIdentifier == receivedEvent.PriceEpisodeIdentifier &&
                           expectedEvent.Amount == receivedEvent.AmountDue &&
                           TestSession.GetLearner(expectedEvent.LearnerId).LearnRefNumber == receivedEvent.Learner?.ReferenceNumber &&
                           expectedEvent.Type == receivedEvent.Type &&
                           expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period;
                });
            }).ToList();

            var allFound = matchedReceivedEvents.Count == expectedPaymentsEvents.Count;
            var nothingExtra = sessionEvents.Count == matchedReceivedEvents.Count;

#if DEBUG
            if ((!allFound || !nothingExtra) && trace)
            {
                if (!allFound)
                {
                    Debug.WriteLine("Did not find all expected events. Trace:");
                    TraceMismatch(expectedPaymentsEvents.ToList(), sessionEvents);
                }

                if (!nothingExtra)
                {
                    var unexpected = sessionEvents.Where(e => !matchedReceivedEvents.Contains(e)).ToList();
                    Debug.WriteLine($"{unexpected.Count} unexpected events:");
                    for (var i = 0; i < unexpected.Count; i++)
                    {
                        var e = unexpected[i];
                        Debug.WriteLine($"{i + 1}: PE:{e.PriceEpisodeIdentifier}, AmountDue:{e.AmountDue}, LearnRefNumber:{e.Learner.ReferenceNumber}, Type:{e.Type}, DeliveryPeriod:{e.DeliveryPeriod.Identifier}, CollectionPeriod:{e.CollectionPeriod.Name}");
                    }
                }
            }
#endif
            return allFound && nothingExtra;
        }

        private void TraceMismatch(IList<OnProgrammePaymentDue> expectedPaymentsEvents, IList<ApprenticeshipContractType2PaymentDueEvent> receivedEvents)
        {
            for (var i = 0; i < expectedPaymentsEvents.Count; i++)
            {
                var expectedEvent = expectedPaymentsEvents[i];
                for (var k = 0; k < receivedEvents.Count; k++)
                {
                    var receivedEvent = receivedEvents[k];
                    var mismatchedFields = new List<string>();
                    var learnerReference = TestSession.GetLearner(expectedEvent.LearnerId).LearnRefNumber;

                    if (expectedEvent.PriceEpisodeIdentifier != receivedEvent.PriceEpisodeIdentifier) mismatchedFields.Add($" PriceEpisodeIdentifier({expectedEvent.PriceEpisodeIdentifier}!={receivedEvent.PriceEpisodeIdentifier})");
                    if (expectedEvent.Amount != receivedEvent.AmountDue) mismatchedFields.Add($" Amount({expectedEvent.Amount}!={receivedEvent.AmountDue})");
                    if (learnerReference != receivedEvent.Learner?.ReferenceNumber) mismatchedFields.Add($"LearnRefNumber({learnerReference}!={receivedEvent.Learner?.ReferenceNumber})");
                    if (expectedEvent.Type != receivedEvent.Type) mismatchedFields.Add($"Type({expectedEvent.Type}!={receivedEvent.Type})");
                    if (expectedEvent.DeliveryPeriod != receivedEvent.DeliveryPeriod?.Period) mismatchedFields.Add($"Period({expectedEvent.DeliveryPeriod}!={receivedEvent.DeliveryPeriod?.Period})");
                    if (!receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)) mismatchedFields.Add($"CollectionPeriod({receivedEvent.CollectionPeriod} does not contain {CollectionYear})");

                    if (mismatchedFields.Count == 0)
                        Debug.WriteLine($"Event {i + 1} of {expectedPaymentsEvents.Count}: match {k + 1}");
                    else
                        Debug.WriteLine($"Event {i + 1} of {expectedPaymentsEvents.Count}: mismatch {k + 1} on {string.Join(",", mismatchedFields)}");
                }
            }
        }

        [Given(@"the following contract type (.*) On Programme earnings are provided:")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsAreProvided(int p0, Table table)
        {
            var allEarnings = table.CreateSet<OnProgrammeEarning>().ToArray();

            Act2EarningEvents = new List<ApprenticeshipContractType2EarningEvent>();

            // create separate earning for each mentioned learner

            foreach (var learnerId in allEarnings.Select(e => e.LearnerId).Distinct())
            {
                var learnerEarnings = allEarnings.Where(e => e.LearnerId == learnerId).ToList();
                var testLearner = TestSession.GetLearner(learnerId); 
                Act2EarningEvents.Add(new ApprenticeshipContractType2EarningEvent
                {
                    Ukprn = TestSession.Ukprn,
                    IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                    JobId = TestSession.JobId,
                    CollectionPeriod = Model.Core.CollectionPeriod.CreateFromAcademicYearAndPeriod(CollectionYear, CollectionPeriod),
                    Learner = new Learner
                    {
                        ReferenceNumber = testLearner.LearnRefNumber,                        
                        Uln = testLearner.Uln
                    },
                    LearningAim = new LearningAim
                    {
                        FrameworkCode = testLearner.Course.FrameworkCode,
                        FundingLineType = testLearner.Course.FundingLineType,
                        Reference = testLearner.Course.LearnAimRef,
                        PathwayCode = testLearner.Course.PathwayCode,
                        StandardCode = testLearner.Course.StandardCode,
                        ProgrammeType = testLearner.Course.ProgrammeType
                    },
                    OnProgrammeEarnings = new ReadOnlyCollection<Model.Core.OnProgramme.OnProgrammeEarning>(
                        learnerEarnings.GroupBy(e => e.Type).Select(group =>
                            new Model.Core.OnProgramme.OnProgrammeEarning
                            {
                                Type = group.Key,
                                Periods = new ReadOnlyCollection<EarningPeriod>(group.Select(e => new EarningPeriod
                                {
                                    Period = e.DeliveryPeriod,
                                    Amount = e.Amount,
                                    PriceEpisodeIdentifier = e.PriceEpisodeIdentifier
                                }).ToList())
                            }).ToArray()),
                    CollectionYear = CollectionYear,
                    SfaContributionPercentage = SfaContributionPercentage,
                });
            }

        }


        [Given(@"the payments are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            SetCurrentCollectionYear();
        }

        [Given(@"the current collection period is R(.*)")]
        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }


        private List<ApprenticeshipContractType2EarningEvent> Act2EarningEvents
        {
            get => Get<List<ApprenticeshipContractType2EarningEvent>>();
            set => Set(value);
        }
    }
}