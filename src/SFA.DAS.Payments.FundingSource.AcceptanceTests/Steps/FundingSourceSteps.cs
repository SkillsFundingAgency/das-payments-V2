﻿using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Data;
using SFA.DAS.Payments.FundingSource.AcceptanceTests.Handlers;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
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
            TestSession.Learners.Clear();
            TestSession.Learners.Add(TestSession.GenerateLearner(TestSession.Ukprn));
        }

        [Given(@"the SFA contribution percentage is (.*)%")]
        public void GivenTheSFAContributionPercentageIs(decimal sfaContribution)
        {
            SfaContributionPercentage = sfaContribution / 100;
        }

        [Given(@"the required payments component generates the following contract type (.*) payable earnings:")]
        public void GivenTheRequiredPaymentsComponentGeneratesTheFollowingContractTypePayableEarnings(ContractType contractType, Table payments)
        {
            ContractType = contractType;
            RequiredPayments = payments.CreateSet<RequiredPayment>().ToList();
        }

        [Given(@"following learners are undertaking training with a training provider")]
        public void GivenFollowingLearnersAreUndertakingTrainingWithATrainingProvider(Table table)
        {
            TestSession.Learners.Clear();
            foreach (var row in table.Rows)
            {
                var learner = TestSession.GenerateLearner(TestSession.Ukprn);
                learner.LearnRefNumber = TestSession.LearnRefNumberGenerator.Generate(learner.Ukprn, row["LearnerId"]);
                TestSession.Learners.Add(learner);
            }
        }

        [When(@"required payments event is received")]
        public async Task WhenRequiredPaymentsEventIsReceived()
        {
            var startTime = DateTimeOffset.UtcNow;
            var payments = RequiredPayments.Select(CreateRequiredPaymentEvent).ToList();
            await CreateTestEarningsJob(startTime, payments.Cast<IPaymentsEvent>().ToList());
            foreach (var paymentDue in payments)
            {
                await MessageSession.Send(paymentDue).ConfigureAwait(false);
            }
        }

        [Then(@"the payment source component will generate the following contract type (.*) coinvested payments:")]
        [Then(@"the payment source component will generate the following contract type (.*) payments:")]
        public async Task ThenThePaymentSourceComponentWillGenerateTheFollowingContractTypeCoinvestedPayments(ContractType expectedContractType, Table expectedFundingSourcePaymentTable)
        {
            var expectedFundingSourcePaymentEvents = expectedFundingSourcePaymentTable.CreateSet<FundingSourcePayment>();
            await WaitForIt(() =>
            {
                return expectedFundingSourcePaymentEvents.All(expectedEvent =>
                    FundingSourcePaymentEventHandler.ReceivedEvents.Any(receivedEvent =>
                         expectedContractType == receivedEvent.ContractType
                         && TestSession.Learner.LearnRefNumber == receivedEvent?.Learner?.ReferenceNumber
                         && TestSession.Ukprn == receivedEvent.Ukprn
                         && expectedEvent.PriceEpisodeIdentifier == receivedEvent.PriceEpisodeIdentifier
                         && expectedEvent.DeliveryPeriod == receivedEvent.DeliveryPeriod
                         && expectedEvent.Type == receivedEvent.TransactionType
                         && expectedEvent.FundingSourceType == receivedEvent.FundingSourceType
                         && expectedEvent.Amount == receivedEvent.AmountDue
                   ));


            }, "Failed to find all the funding source payment events");
        }

        private CalculatedRequiredOnProgrammeAmount BuildApprenticeshipContractTypeRequiredPaymentEvent(RequiredPayment requiredPayment)
        {
            var paymentEvent = ContractType == ContractType.Act1
                ? (CalculatedRequiredOnProgrammeAmount)new CalculatedRequiredLevyAmount()
                : new CalculatedRequiredCoInvestedAmount();

            MapCommon(requiredPayment, paymentEvent);
            paymentEvent.OnProgrammeEarningType = (OnProgrammeEarningType)requiredPayment.Type;
            paymentEvent.SfaContributionPercentage = SfaContributionPercentage;
            return paymentEvent;
        }

        private CalculatedRequiredIncentiveAmount BuildIncentiveRequiredPaymentEvent(RequiredPayment requiredPayment)
        {
            var paymentEvent = new CalculatedRequiredIncentiveAmount();
            MapCommon(requiredPayment, paymentEvent);
            paymentEvent.Type = (IncentivePaymentType)requiredPayment.Type;
            paymentEvent.ContractType = (ContractType)ContractType;
            return paymentEvent;
        }

        private void MapCommon(RequiredPayment requiredPayment, PeriodisedRequiredPaymentEvent paymentEvent)
        {
            paymentEvent.Learner = TestSession.Learner.ToLearner();
            paymentEvent.Ukprn = TestSession.Ukprn;
            paymentEvent.AmountDue = requiredPayment.Amount;
            paymentEvent.JobId = TestSession.JobId;
            paymentEvent.EventTime = DateTimeOffset.UtcNow;
            paymentEvent.CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(AcademicYear, CollectionPeriod);
            paymentEvent.DeliveryPeriod = requiredPayment.DeliveryPeriod;
            paymentEvent.IlrSubmissionDateTime = TestSession.IlrSubmissionTime;
            paymentEvent.LearningAim = TestSession.Learner.Course.ToLearningAim();
            paymentEvent.PriceEpisodeIdentifier = requiredPayment.PriceEpisodeIdentifier;
        }

        private PeriodisedRequiredPaymentEvent CreateRequiredPaymentEvent(RequiredPayment requiredPayment)
        {
            PeriodisedRequiredPaymentEvent paymentEvent;
            switch (requiredPayment.Type)
            {
                case TransactionType.Learning:
                case TransactionType.Completion:
                case TransactionType.Balancing:
                    paymentEvent = BuildApprenticeshipContractTypeRequiredPaymentEvent(requiredPayment);
                    break;
                default:
                    paymentEvent = BuildIncentiveRequiredPaymentEvent(requiredPayment);
                    break;
            }
            return paymentEvent;
        }
    }
}
