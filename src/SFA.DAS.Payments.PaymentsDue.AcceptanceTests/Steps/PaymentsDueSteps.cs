using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class PaymentsDueSteps : StepsBase
    {
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

            this.WaitForIt(() =>
            {
                return expectedPaymentsEvents.All(payment => act == 1 ? Act1Matcher(payment) : Act2Matcher(payment));
            }, "Failed to find all the payment due events");
        }

        private bool Act1Matcher(OnProgrammePaymentDue expectedEvent)
        {
            throw new NotImplementedException();
        }

        private bool Act2Matcher(OnProgrammePaymentDue expectedEvent)
        {
            return ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Any(receivedEvent =>
                expectedEvent.Amount == receivedEvent.AmountDue &&
                TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber &&
                expectedEvent.Type == receivedEvent.Type &&
                TestSession.Ukprn == receivedEvent.Ukprn &&
                expectedEvent.Delivery_Period == receivedEvent.DeliveryPeriod?.Period &&
                receivedEvent.CollectionPeriod == new CalendarPeriod(CollectionYear, CollectionPeriod));
        }


        [Given(@"the following contract type (.*) On Programme earnings are provided:")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsAreProvided(int p0, Table table)
        {
            var rawEarnings = table.CreateSet<ContractTypeEarning>().ToArray();

            this.Act2EarningEvents = new List<ApprenticeshipContractType2EarningEvent>
            {
                new ApprenticeshipContractType2EarningEvent
                {
                    CollectionPeriod = new CalendarPeriod(this.CollectionYear, this.CollectionPeriod),
                    Learner = new Learner
                    {
                        ReferenceNumber = this.TestSession.Learner.LearnRefNumber,
                        Ukprn = this.TestSession.Ukprn,
                        Uln = this.TestSession.Learner.Uln
                    },
                    LearningAim = new LearningAim
                    {
                        AgreedPrice = this.TestSession.Learner.Course.AgreedPrice,
                        FrameworkCode = this.TestSession.Learner.Course.FrameworkCode,
                        FundingLineType = this.TestSession.Learner.Course.FundingLineType,
                        Reference = this.TestSession.Learner.Course.LearnAimRef,
                        PathwayCode = this.TestSession.Learner.Course.PathwayCode,
                        StandardCode = this.TestSession.Learner.Course.StandardCode,
                        ProgrammeType = this.TestSession.Learner.Course.ProgrammeType
                    },
                    OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(
                        rawEarnings.GroupBy(e => e.Type).Select(group =>
                            new OnProgrammeEarning
                            {
                                Type = group.Key,
                                Periods = new ReadOnlyCollection<EarningPeriod>(group.Select(e => new EarningPeriod
                                {
                                    Period = new CalendarPeriod(this.CollectionYear, e.Delivery_Period),
                                    Amount = e.Amount,
                                    PriceEpisodeIdentifier = e.PriceEpisodeIdentifier
                                }).ToList())
                            }).ToArray())
                }
            };

            //yesScenarioCtx[$"ContractType{this.ContractType}OnProgrammeEarningsLearning"] = earning;
        }

        private List<ApprenticeshipContractType2EarningEvent> Act2EarningEvents
        {
            get => Get<List<ApprenticeshipContractType2EarningEvent>>();
            set => Set(value);
        }

        protected PaymentsDueSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}