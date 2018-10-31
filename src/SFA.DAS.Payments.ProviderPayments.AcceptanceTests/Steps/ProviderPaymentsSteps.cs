﻿using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.AcceptanceTests.Handlers;
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
            // Use Auto Generated Learning Ref
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
            var payments = FundingSourcePayments.Select(CreateFundingSourcePaymentEvent).ToList();
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
            var savedPayments = await GetPayemntsAsync(TestSession.JobId);

            WaitForIt(() =>
            {
                return expectedPaymentsEvent.All(expectedEvent =>
                    savedPayments.Any(payment =>
                        expectedContract == payment.ContractType
                        && TestSession.Learner.LearnRefNumber == payment.LearnerReferenceNumber
                        && TestSession.Ukprn == payment.Ukprn
                        && expectedEvent.DeliveryPeriod == payment.DeliveryPeriod?.Period
                        && expectedEvent.Type == (OnProgrammeEarningType)payment.TransactionType
                        && expectedEvent.FundingSourceType == payment.FundingSource
                        && expectedEvent.Amount == payment.Amount
                    ));


            }, "Failed to find all payment in database");

        }

        [Then(@"at month end the provider payments service will publish the following payments")]
        public async Task ThenAtMonthEndTheProviderPaymentsServiceWillPublishTheFollowingPayments(Table expectedProviderPayments)
        {
            await MessageSession.Send(new MonthEndEvent
            {
                JobId = TestSession.JobId,
                CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod)
            }).ConfigureAwait(false);


            var expectedProviderPaymentEvents = expectedProviderPayments.CreateSet<FundingSourcePayment>();
  
            WaitForIt(() =>
            {
                return expectedProviderPaymentEvents.All(expectedEvent =>
                    ProviderPaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                        ContractType == receivedEvent.ContractType
                        && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                        && TestSession.Ukprn == receivedEvent.Ukprn
                        && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod?.Period
                        && expectedEvent.Type == (OnProgrammeEarningType)receivedEvent.ContractType
                        && expectedEvent.FundingSourceType == receivedEvent.FundingSourceType
                        && expectedEvent.Amount == receivedEvent.AmountDue
                        && TestSession.JobId == receivedEvent.JobId
                    ));


            }, "Failed to find all the provider payment events");

        }

        private FundingSourcePaymentEvent CreateFundingSourcePaymentEvent(FundingSourcePayment fundingSourcePayment)
        {
            FundingSourcePaymentEvent paymentEvent;

            switch (fundingSourcePayment.FundingSourceType)
            {
                case Model.Core.Entities.FundingSourceType.CoInvestedSfa:
                    paymentEvent = new SfaCoInvestedFundingSourcePaymentEvent();
                    break;
                case Model.Core.Entities.FundingSourceType.CoInvestedEmployer:
                    paymentEvent = new EmployerCoInvestedFundingSourcePaymentEvent();
                    break;
                default:
                    //TODO Implement other FundingSourceTypes
                    throw new NotImplementedException("Unhandled Funding Source Type");
            }

            paymentEvent.FundingSourceType = fundingSourcePayment.FundingSourceType;
            paymentEvent.IlrSubmissionDateTime = DateTime.UtcNow;
            paymentEvent.ContractType = (Model.Core.Entities.ContractType)ContractType;
            paymentEvent.Learner = TestSession.Learner.ToLearner();
            paymentEvent.Ukprn = TestSession.Ukprn;
            paymentEvent.OnProgrammeEarningType = fundingSourcePayment.Type;
            paymentEvent.AmountDue = fundingSourcePayment.Amount;
            paymentEvent.JobId = TestSession.JobId;
            paymentEvent.EventTime = DateTimeOffset.UtcNow;
            paymentEvent.SfaContributionPercentage = SfaContributionPercentage;
            paymentEvent.CollectionPeriod = new CalendarPeriod(GetYear(CollectionPeriod, CollectionYear).ToString(), CollectionPeriod);
            paymentEvent.DeliveryPeriod = new CalendarPeriod(GetYear(fundingSourcePayment.DeliveryPeriod, CollectionYear).ToString(), fundingSourcePayment.DeliveryPeriod);
            paymentEvent.LearningAim = TestSession.Learner.Course.ToLearningAim();
            paymentEvent.PriceEpisodeIdentifier = "P1";
            return paymentEvent;
        }

    }
}