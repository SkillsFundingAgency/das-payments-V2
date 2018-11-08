using System;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Handlers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
                await MessageSession.Send(act2EarningEvent);
            }
        }

        [Then(@"the payments due component will generate the following contract type (.*) payments due:")]
        public void ThenThePaymentsDueComponentWillGenerateTheFollowingContractTypePaymentsDue(int act, Table table)
        {
            var expectedPaymentsEvents = table.CreateSet<OnProgrammePaymentDue>().ToList();
            WaitForIt(() => MatchPaymentDue(expectedPaymentsEvents), "Failed to find all the payment due events");
        }

        private bool MatchPaymentDue(IReadOnlyCollection<OnProgrammePaymentDue> expectedPaymentsEvents)
        {
            var matchedReceivedEvents = ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Where(receivedEvent =>
            {
                return expectedPaymentsEvents.Any(expectedEvent =>
                {
                    return expectedEvent.PriceEpisodeIdentifier == receivedEvent.PriceEpisodeIdentifier &&
                           expectedEvent.Amount == receivedEvent.AmountDue &&
                           TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber &&
                           expectedEvent.Type == receivedEvent.Type &&
                           TestSession.Ukprn == receivedEvent.Ukprn &&
                           expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period &&
                           receivedEvent.CollectionPeriod.GetCollectionYear() == CollectionYear && 
                           receivedEvent.CollectionPeriod.Period == CollectionPeriod;
                });
            }).ToList();

            if (!trace && matchedReceivedEvents.Count < expectedPaymentsEvents.Count)
                return false;

            var unexpected = ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents
                .Where(receivedEvent => !matchedReceivedEvents.Contains(receivedEvent) &&
                                         TestSession.Ukprn == receivedEvent.Ukprn).ToList();
            return matchedReceivedEvents.Count == expectedPaymentsEvents.Count && unexpected.Count == 0;
        }

        [Given(@"the following contract type (.*) On Programme earnings are provided:")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsAreProvided(int p0, Table table)
        {
            var allEarnings = table.CreateSet<OnProgrammeEarning>().ToArray();

            Act2EarningEvents = new List<ApprenticeshipContractType2EarningEvent>
            {
                new ApprenticeshipContractType2EarningEvent
                {
                    Ukprn = TestSession.Ukprn,
                    JobId = TestSession.JobId,
                    EventTime = DateTimeOffset.UtcNow,
                    CollectionPeriod = new CalendarPeriod(CollectionYear, CollectionPeriod),
                    IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                    Learner =
                        new Learner
                        {
                            ReferenceNumber = TestSession.Learner.LearnRefNumber, Uln = TestSession.Learner.Uln
                        },
                    LearningAim = new LearningAim
                    {
                        AgreedPrice = TestSession.Learner.Course.AgreedPrice,
                        FrameworkCode = TestSession.Learner.Course.FrameworkCode,
                        FundingLineType = TestSession.Learner.Course.FundingLineType,
                        Reference = TestSession.Learner.Course.LearnAimRef,
                        PathwayCode = TestSession.Learner.Course.PathwayCode,
                        StandardCode = TestSession.Learner.Course.StandardCode,
                        ProgrammeType = TestSession.Learner.Course.ProgrammeType
                    },
                    OnProgrammeEarnings = new ReadOnlyCollection<Model.Core.OnProgramme.OnProgrammeEarning>(
                        allEarnings.GroupBy(e => e.Type).Select(group =>
                            new Model.Core.OnProgramme.OnProgrammeEarning
                            {
                                Type = @group.Key,
                                Periods = new ReadOnlyCollection<EarningPeriod>(@group.Select(e => new EarningPeriod
                                {
                                    Period = e.DeliveryPeriod,
                                    Amount = e.Amount,
                                    PriceEpisodeIdentifier = e.PriceEpisodeIdentifier
                                }).ToList())
                            }).ToArray()),
                    CollectionYear = CollectionYear,
                    SfaContributionPercentage = SfaContributionPercentage,
                }
            };

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