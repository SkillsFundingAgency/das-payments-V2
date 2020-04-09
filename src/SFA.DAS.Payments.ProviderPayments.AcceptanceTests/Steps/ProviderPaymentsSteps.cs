using NServiceBus;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
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
            TestSession.Learners.Add(TestSession.GenerateLearner(TestSession.Ukprn));
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
        public void GivenTheFundingSourceServiceGeneratesTheFollowingContractTypePayments(ContractType contractType, Table payments)
        {
            ContractType = contractType;
            FundingSourcePayments = payments.CreateSet<FundingSourcePayment>().ToList();
        }

        [When(@"the funding source payments event are received")]
        public async Task WhenTheFundingSourcePaymentsEventAreReceivedAsync()
        {
            var startTime = DateTimeOffset.UtcNow;
            var submissionTime = TestSession.IlrSubmissionTime;
            var payments = FundingSourcePayments.Select(p => CreateFundingSourcePaymentEvent(p, submissionTime)).ToList();
            await CreateTestEarningsJob(startTime, payments.Cast<IPaymentsEvent>().ToList());
            foreach (var payment in payments)
            {
                await MessageSession.Send(payment).ConfigureAwait(false);
            }
        }

        [When(@"the period closes and month end processing begins")]
        public async Task WhenThePeriodClosesAndMonthEndProcessingBegins()
        {
            await SendMonthEndEvent().ConfigureAwait(false);
        }
        
        [Then(@"the provider payments service will publish the following payments")]
        public async Task ThenTheProviderPaymentsServiceWillPublishTheFollowingPayments(Table expectedProviderPayments)
        {
            var expectedProviderPaymentEvents = expectedProviderPayments.CreateSet<FundingSourcePayment>();

            await WaitForIt(() =>
            {
                return expectedProviderPaymentEvents.All(expectedEvent =>
                    ProviderPaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                            ContractType == receivedEvent.ContractType
                            && TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber
                            && TestSession.Ukprn == receivedEvent.Ukprn
                            && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod
                            && expectedEvent.Type == receivedEvent.TransactionType
                            && expectedEvent.FundingSourceType == receivedEvent.FundingSourceType
                            && expectedEvent.Amount == receivedEvent.AmountDue
                    
                    ));
            }, $"Failed to find all the provider payment events. Found '{ProviderPaymentEventHandler.ReceivedEvents.Count}' events ");
        }


        [Then(@"the provider payments service will store the following payments:")]
        public async Task ThenTheProviderPaymentsServiceWillStoreTheFollowingPaymentsAsync(Table expectedPaymentsTable)
        {
            var expectedContract = (Model.Core.Entities.ContractType)ContractType;
            var expectedPaymentsEvent = expectedPaymentsTable.CreateSet<FundingSourcePayment>();

            await WaitForIt(async () =>
            {
                Console.WriteLine($"Looking for payments for job {TestSession.JobId}. Time: {DateTime.Now:G}");
                var savedPayments =  await GetPaymentsAsync(TestSession.JobId);
                Console.WriteLine($"Found {savedPayments.Count} payments. Time: {DateTime.Now:G}");
                var found = expectedPaymentsEvent.All(expectedEvent =>
                    savedPayments.Any(payment =>
                        expectedContract == payment.ContractType
                        && TestSession.Learner.LearnRefNumber == payment.LearnerReferenceNumber
                        && TestSession.Ukprn == payment.Ukprn
                        && expectedEvent.DeliveryPeriod == payment.DeliveryPeriod
                        && expectedEvent.Type == payment.TransactionType
                        && expectedEvent.FundingSourceType == payment.FundingSource
                        && expectedEvent.Amount == payment.Amount
                    ));
                return found;
            }, "Failed to find all payment in database.");
        }

        [Then(@"at month end the provider payments service will publish the following payments")]
        public async Task ThenAtMonthEndTheProviderPaymentsServiceWillPublishTheFollowingPayments(Table expectedProviderPayments)
        {
            await SendMonthEndEvent();

            var expectedProviderPaymentEvents = expectedProviderPayments.CreateSet<FundingSourcePayment>();

            await WaitForIt(() =>
            {
                return expectedProviderPaymentEvents.All(expectedEvent =>
                    ProviderPaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                        ContractType == receivedEvent.ContractType
                        && TestSession.Learner.LearnRefNumber == receivedEvent.Learner?.ReferenceNumber
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod
                        && expectedEvent.Type == receivedEvent.TransactionType
                        && expectedEvent.FundingSourceType == receivedEvent.FundingSourceType
                        && expectedEvent.Amount == receivedEvent.AmountDue
                        //&& TestSession.JobId == receivedEvent.JobId
                    ));
            }, "Failed to find all the provider payment events");
        }
        
        private async Task SendMonthEndEvent()
        {
            MonthEndJobId = TestSession.GenerateId();
            Console.WriteLine($"Month end job id: {MonthEndJobId}");
            var monthEndCommand = new ProcessProviderMonthEndCommand
            {
                Ukprn = TestSession.Ukprn,
                JobId = MonthEndJobId,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(AcademicYear, CollectionPeriod),
            };
            await CreateJob(DateTimeOffset.UtcNow, new List<GeneratedMessage>
                {
                    new GeneratedMessage
                    {
                        StartTime = monthEndCommand.RequestTime,
                        MessageName = monthEndCommand.GetType().FullName,
                        MessageId = monthEndCommand.CommandId
                    }
                }, 
                JobType.ComponentAcceptanceTestMonthEndJob);
            await MessageSession.Send(monthEndCommand).ConfigureAwait(false);
        }
    }
}