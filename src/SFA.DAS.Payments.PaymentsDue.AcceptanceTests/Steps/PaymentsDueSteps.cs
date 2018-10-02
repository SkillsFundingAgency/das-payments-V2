using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using TechTalk.SpecFlow;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Handlers;
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
                var outcome = new StringBuilder();
                if (expectedPaymentsEvents.Count < ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Count)
                {
                    outcome.AppendLine($"No match. Received {ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Count} out of {expectedPaymentsEvents.Count}.");
                    return false;
                }

                outcome.AppendLine($"Count match. Comparing items. Received {ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Count} out of {expectedPaymentsEvents.Count}");
                var result = expectedPaymentsEvents.All(payment => act == 1 ? Act1Matcher(payment, outcome) : Act2Matcher(payment, outcome));
                Debug.Write(outcome);
                return result;
            }, "Failed to find all the payment due events");
        }

        private bool Act1Matcher(OnProgrammePaymentDue expectedEvent, StringBuilder outcome)
        {
            throw new NotImplementedException();
        }

        private bool Act2Matcher(OnProgrammePaymentDue expectedEvent, StringBuilder outcome)
        {
            var result = ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents.Any(receivedEvent =>
                expectedEvent.Amount == receivedEvent.AmountDue &&
                TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber &&
                expectedEvent.Type == receivedEvent.Type &&
                TestSession.Ukprn == receivedEvent.Ukprn &&
                expectedEvent.Delivery_Period == receivedEvent.DeliveryPeriod?.Period &&
                receivedEvent.CollectionPeriod == new CalendarPeriod(CollectionYear, CollectionPeriod));

            if (result)
                outcome.AppendLine($"Found match for AmountDue:{expectedEvent.Amount}, LearnRefNo:{TestSession.Learner.LearnRefNumber}, Type:{expectedEvent.Type}, Ukprn:{TestSession.Ukprn}, DeliveryPeriod:{expectedEvent.Delivery_Period}, Collection Period:{new CalendarPeriod(CollectionYear, CollectionPeriod).Name}");
            else
            {
                outcome.AppendLine($"Not found match for AmountDue:{expectedEvent.Amount}, LearnRefNo:{TestSession.Learner.LearnRefNumber}, Type:{expectedEvent.Type}, Ukprn:{TestSession.Ukprn}, DeliveryPeriod:{expectedEvent.Delivery_Period}, Collection Period:{new CalendarPeriod(CollectionYear, CollectionPeriod).Name}");
                outcome.AppendLine("Candidates:");
                foreach (var receivedEvent in ApprenticeshipContractType2PaymentDueEventHandler.ReceivedEvents)
                {                    
                    outcome.Append($"------------------- AmountDue:{receivedEvent.AmountDue}, LearnRefNo:{receivedEvent.Learner?.ReferenceNumber}, Type:{receivedEvent.Type}, Ukprn:{receivedEvent.Ukprn}, DeliveryPeriod:{receivedEvent.DeliveryPeriod?.Period}, CollectionPeriod:{receivedEvent.CollectionPeriod.Name}. Mismatch on ");
                    if (expectedEvent.Amount != receivedEvent.AmountDue) outcome.Append("AmountDue ");
                    if (TestSession.Learner.LearnRefNumber != receivedEvent.Learner?.ReferenceNumber) outcome.Append("LearnRefNumber ");
                    if (expectedEvent.Type != receivedEvent.Type) outcome.Append("Type ");
                    if (TestSession.Ukprn != receivedEvent.Ukprn) outcome.Append("Ukprn ");
                    if (expectedEvent.Delivery_Period != receivedEvent.DeliveryPeriod?.Period) outcome.Append("DeliveryPeriod ");
                    if (receivedEvent.CollectionPeriod != new CalendarPeriod(CollectionYear, CollectionPeriod)) outcome.Append("CollectionPeriod");
                    outcome.AppendLine();
                }
            }

            return result;
        }

        [Given(@"the following contract type (.*) On Programme earnings are provided in the latest ILR for the current academic year:")]
        public void GivenTheFollowingContractTypeOnProgrammeEarningsAreProvidedInTheLatestILRForTheCurrentAcademicYear(int p0, Table table)
        {
            var rawEarnings = table.CreateSet<ContractTypeEarning>().ToArray();
            var transactionType = rawEarnings[0].Type;

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

        public List<ApprenticeshipContractType2EarningEvent> Act2EarningEvents
        {
            get => Get<List<ApprenticeshipContractType2EarningEvent>>();
            set => Set(value);
        }

        protected PaymentsDueSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }
    }
}