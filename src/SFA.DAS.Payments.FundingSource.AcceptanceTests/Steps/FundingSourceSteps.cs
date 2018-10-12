using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        [Given(@"the payments are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            SetCurrentCollectionYear();
        }

        [Given(@"the current processing period is (.*)")]
        [Given(@"the current collection period is R(.*)")]
        public void GivenTheCurrentProcessingPeriodIs(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"a learner is undertaking a training with a training provider")]
        public void GivenALearnerIsUndertakingATrainingWithATrainingProvider()
        {
            // Use Auto Generated Learning Ref
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
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
            var expectedFundingSourcePaymentEvents = expectedFundingSourcePaymentTable.CreateSet<FundingSourcePayment>();
            WaitForIt(() =>
            {
                return expectedFundingSourcePaymentEvents.All(expectedEvent =>
                    CoInvestedFundingSourceHandler.ReceivedEvents.Any(receivedEvent =>
                         expectedContractType == receivedEvent.ContractType
                         && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                         && TestSession.Ukprn == receivedEvent.Ukprn
                         && expectedEvent.PriceEpisodeIdentifier == receivedEvent.PriceEpisodeIdentifier
                         && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period
                         && expectedEvent.Type == receivedEvent.OnProgrammeEarningType
                         && expectedEvent.FundingSourceType == receivedEvent.FundingSourceType
                         && expectedEvent.Amount == receivedEvent.AmountDue
                   ));


            }, "Failed to find all the funding source payment events");
        }

        private ApprenticeshipContractTypeRequiredPaymentEvent CreateRequiredPaymentEvent(RequiredPayment requiredPayment)
        {
            var paymentEvent = ContractType == 1
                ? (ApprenticeshipContractTypeRequiredPaymentEvent)new ApprenticeshipContractType1RequiredPaymentEvent()
                : new ApprenticeshipContractType2RequiredPaymentEvent();

            paymentEvent.Learner = TestSession.Learner.ToLearner();
            paymentEvent.Ukprn = TestSession.Ukprn;
            paymentEvent.OnProgrammeEarningType = requiredPayment.Type;
            paymentEvent.AmountDue = requiredPayment.Amount;
            paymentEvent.JobId = TestSession.JobId;
            paymentEvent.EventTime = DateTimeOffset.UtcNow;
            paymentEvent.SfaContributionPercentage = SfaContributionPercentage;
            paymentEvent.CollectionPeriod =new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod);
            paymentEvent.DeliveryPeriod = new CalendarPeriod(GetYear(requiredPayment.DeliveryPeriod, CollectionYear).ToString(), requiredPayment.DeliveryPeriod);
          
            paymentEvent.LearningAim = TestSession.Learner.Course.ToLearningAim();
            paymentEvent.PriceEpisodeIdentifier = requiredPayment.PriceEpisodeIdentifier;
            return paymentEvent;
        }
    }
}
