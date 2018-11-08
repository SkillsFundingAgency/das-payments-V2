using NServiceBus;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers;
using SFA.DAS.Payments.ProviderPayments.Messages.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Steps
{
    [Binding]
    public class ProviderPaymentsSteps : ProviderPaymentsStepsBase
    {
        public ProviderPaymentsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"a learner is undertaking a training with a training provider")]
        public void GivenALearnerIsUndertakingATrainingWithATrainingProvider()
        {
            TestSession.Learners.Clear();
            TestSession.Learners.Add(TestSession.GenerateLearner());
        }

        [Given(@"the payments are for the current collection year")]
        public void GivenThePaymentsAreForTheCurrentCollectionYear()
        {
            SetCurrentCollectionYear();
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
        public void GivenTheSFAContributionPercentageIs(decimal sfaContribution)
        {
            SfaContributionPercentage = sfaContribution / 100;
        }

        [Given(@"the current collection period is R(.*)")]
        public void GivenTheCurrentCollectionPeriodIsR(byte period)
        {
            CollectionPeriod = period;
        }

        [Given(@"the funding source service generates the following contract type (.*) payments:")]
        public void GivenTheFundingSourceServiceGeneratesTheFollowingContractTypePayments(byte contractType, Table payments)
        {
            ContractType = contractType;
            FundingSourcePayments = payments.CreateSet<FundingSourcePayment>().ToList();
        }

        [When(@"the funding source payments event are received")]
        public async Task WhenTheFundingSourcePaymentsEventAreReceivedAsync()
        {
            var submissionTime = DateTime.UtcNow;
            var payments = FundingSourcePayments.Select(p => CreateFundingSourcePaymentEvent(p, submissionTime)).ToList();
            foreach (var payment in payments)
            {
                await MessageSession.Send(payment).ConfigureAwait(false);
            }
        }

        [Then(@"the provider payments service will store the following payments:")]
        public async Task ThenTheProviderPaymentsServiceWillStoreTheFollowingPaymentsAsync(Table expectedPaymentsTable)
        {
            var expectedContract = (Model.Core.Entities.ContractType)ContractType;
            var expectedPaymentsEvent = expectedPaymentsTable.CreateSet<FundingSourcePayment>();

            await WaitForItAsync(() =>
            {
                var savedPayments =  GetPaymentsAsync(TestSession.JobId).Result;

                var found = expectedPaymentsEvent.All(expectedEvent =>
                    savedPayments.Any(payment =>
                        expectedContract == payment.ContractType
                        && TestSession.Learner.LearnRefNumber == payment.LearnerReferenceNumber
                        && TestSession.Ukprn == payment.Ukprn
                        && expectedEvent.DeliveryPeriod == payment.DeliveryPeriod?.Period
                        && expectedEvent.Type == (OnProgrammeEarningType)payment.TransactionType
                        && expectedEvent.FundingSourceType == payment.FundingSource
                        && expectedEvent.Amount == payment.Amount
                    ));
                return found;
            }, "Failed to find all payment in database");
        }

        [Then(@"at month end the provider payments service will publish the following payments")]
        public async Task ThenAtMonthEndTheProviderPaymentsServiceWillPublishTheFollowingPayments(Table expectedProviderPayments)
        {
            await SendMonthEndEvent();

            var expectedProviderPaymentEvents = expectedProviderPayments.CreateSet<FundingSourcePayment>();

            await WaitForItAsync(() =>
            {
                return expectedProviderPaymentEvents.All(expectedEvent =>
                    CoInvestedProviderPaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                        ContractType == receivedEvent.ContractType
                        && TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period
                        && expectedEvent.Type == (OnProgrammeEarningType)receivedEvent.TransactionType
                        && expectedEvent.FundingSourceType == receivedEvent.FundingSourceType
                        && expectedEvent.Amount == receivedEvent.AmountDue
                        && TestSession.JobId == receivedEvent.JobId
                    ));


            }, "Failed to find all the provider payment events");

        }


        private async Task SendMonthEndEvent()
        {
            await MessageSession.Send(new PerformMonthEndProcessingCommand()
            {
                JobId = TestSession.JobId,
                CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod)
            }).ConfigureAwait(false);

        }


    }
}