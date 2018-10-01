using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Steps
{
    [Binding]
    public class FundingSourceSteps : FundingSourceStepBase
    {


        protected FundingSourceSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"the current processing period is (.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"a learner with LearnRefNumber learnref(.*) and Uln (.*) undertaking training with training provider (.*)")]
        public void GivenALearnerWithLearnRefNumberLearnrefAndUlnUndertakingTrainingWithTrainingProvider(int learnerRef, long uln, int provider)
        {
           //TODO map to TestContextProvider
        }

        [Given(@"the SFA contribution percentage is (.*)%""")]
        public void GivenTheSFAContributionPercentageIs(decimal sfaContribution)
        {
            SfaContributionPercentage = sfaContribution / 100;
        }

        [Given(@"the required payments component generates the following contract type (.*) payable earnings:")]
        public void GivenTheRequiredPaymentsComponentGeneratesTheFollowingContractTypePayableEarnings(byte contractType, Table payments)
        {
            ContractType = contractType;
            RequiredPayments = payments.CreateSet<RequiredPayment>().ToList();
        }

        [When(@"required payments event is received")]
        public async Task WhenRequiredPaymentsEventIsReceived()
        {
            var payments = RequiredPayments.Select(CreateRequiredPaymentEvent).ToList();
            foreach (var paymentDue in payments)
            {
                await MessageSession.Send(paymentDue).ConfigureAwait(false);
            }
        }

        [Then(@"the payment source component will generate the following contract type (.*) coinvested payments:")]
        public void ThenThePaymentSourceComponentWillGenerateTheFollowingContractTypeCoinvestedPayments(byte expectedContractType, Table expectedFundingSourcePaymentTable)
        {
            var expectedFundingSourcePaymentEvents = expectedFundingSourcePaymentTable.CreateSet<SfaCoInvestedFundingSourcePaymentEvent>();

            WaitForIt(() =>
            {
                return expectedFundingSourcePaymentEvents.All(expectedEvent =>
                    CoInvestedFundingSourceHandler.ReceivedEvents.Any(receivedEvent =>
                        expectedEvent.AmountDue == receivedEvent.AmountDue
                        && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                        && expectedEvent.OnProgrammeEarningType == receivedEvent.OnProgrammeEarningType
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && expectedEvent.Period == receivedEvent.DeliveryPeriod?.Period
                       // && receivedEvent.CollectionPeriod.Name.Contains(CollectionYear)
                        && receivedEvent.ContractType == expectedContractType)
                    );
            }, "Failed to find all the funding source payment events");
        }

        private ApprenticeshipContractTypeRequiredPaymentEvent CreateRequiredPaymentEvent(RequiredPayment requiredPayment)
        {
            var paymentEvent = ContractType == 1
                ? (ApprenticeshipContractTypeRequiredPaymentEvent)new ApprenticeshipContractType1RequiredPaymentEvent()
                : new ApprenticeshipContractType2RequiredPaymentEvent();

            paymentEvent.Learner = TestSession.Learner.ToLearner();
            paymentEvent.Ukprn = TestSession.Ukprn;
            paymentEvent.OnProgrammeEarningType = (OnProgrammeEarningType)requiredPayment.TransactionType;
            paymentEvent.AmountDue = requiredPayment.Amount;
            paymentEvent.JobId = TestSession.JobId;
            paymentEvent.EventTime = DateTimeOffset.UtcNow;
            paymentEvent.SfaContributionPercentage = SfaContributionPercentage;
           // paymentEvent.CollectionPeriod = new CalendarPeriod { Name = $"{CollectionYear}-R{CollectionPeriod}", Month = GetMonth(CollectionPeriod), Period = CollectionPeriod, Year = GetYear(CollectionPeriod, CollectionYear) };
           // paymentEvent.DeliveryPeriod = new CalendarPeriod { Name = $"{CollectionYear}-R{requiredPayment.Period}", Month = GetMonth(requiredPayment.Period), Period = requiredPayment.Period, Year = GetYear(requiredPayment.Period, CollectionYear) };
            paymentEvent.LearningAim = TestSession.Learner.Course.ToLearningAim();
            paymentEvent.PriceEpisodeIdentifier = requiredPayment.PriceEpisodeIdentifier;
            return paymentEvent;
        }
    }


}
