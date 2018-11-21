using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.Application.Repositories;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class EndToEndSteps : EndToEndStepsBase
    {
        public EndToEndSteps(FeatureContext context) : base(context)
        {
        }

        [Given(@"the provider is providing training for the following learners")]
        public void GivenTheProviderIsProvidingTrainingForTheFollowingLearners(Table table)
        {
            CurrentIlr = table.CreateSet<Training>().ToList();
        }

        [Given(@"the provider previously submitted the following learner details in collection period ""(.*)""")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsInCollectionPeriod(string previousCollectionPeriod, Table table)
        {
            SetCollectionPeriod(previousCollectionPeriod);
            var ilr = table.CreateSet<Training>().ToList();
            PreviousIlr = ilr;
        }

        [Then(@"the following learner earnings should be generated")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            var earnings = table.CreateSet<OnProgrammeEarning>().ToList();
            
            foreach (var training in CurrentIlr)
            {
                var learnerId = training.LearnerId ?? TestSession.Learner.LearnerIdentifier;
                var learner = new FM36Learner {LearnRefNumber = TestSession.GenerateLearnerReference(learnerId)};
                var learnerEarnings = earnings.Where(e => (e.LearnerId ?? TestSession.Learner.LearnerIdentifier) == learnerId).ToList();
                
                PopulateLearner(learner, training, learnerEarnings);

                var command = new ProcessLearnerCommand
                {
                    Learner = learner,
                    CollectionPeriod = CurrentCollectionPeriod.Period,
                    CollectionYear = CollectionYear,
                    Ukprn = TestSession.Ukprn,
                    JobId = TestSession.JobId,
                    IlrSubmissionDateTime = TestSession.IlrSubmissionTime,
                    RequestTime = DateTimeOffset.UtcNow,
                    SubmissionDate = TestSession.IlrSubmissionTime, //TODO: ????                    
                };

                Console.WriteLine($"Sending process learner command to the earning events service. Command: {command.ToJson()}");
                await MessageSession.Send(command);
            }

            await WaitForIt(() => EarningEventMatcher.MatchEarnings(earnings, TestSession), "OnProgrammeEarning event check failure");
        }

        [Then(@"only the following payments will be calculated")]
        public async Task ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = table.CreateSet<Payment>().ToList();
            var matcher = new RequiredPaymentEventMatcher(TestSession, CurrentCollectionPeriod, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Required Payment event check failure");
        }

        [Then(@"no payments will be calculated")]
        public async Task ThenNoPaymentsWillBeCalculated()
        {
            var matcher = new RequiredPaymentEventMatcher(TestSession, CurrentCollectionPeriod);
            await WaitForUnExpected(() => matcher.MatchNoPayments(), "Required Payment event check failure");
        }

        [Then(@"at month end only the following provider payments will be generated")]
        public async Task ThenTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            var monthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = TestSession.Ukprn,
                JobId = TestSession.JobId
            };
            await MessageSession.Send(monthEndCommand);
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            var matcher = new ProviderPaymentEventMatcher(CurrentCollectionPeriod, TestSession, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Provider Payment event check failure");
        }

        [Then(@"no provider payments will be recorded")]
        public async Task ThenNoProviderPaymentsWillBeRecorded()
        {
            var dataContext = Container.Resolve<IPaymentsDataContext>();
            var matcher = new ProviderPaymentModelMatcher(dataContext, TestSession, CurrentCollectionPeriod.Name);
            await WaitForUnExpected(() => matcher.MatchNoPayments(), "Payment history check failure");
        }

        [Then(@"at month end no provider payments will be generated")]
        public async Task ThenAtMonthEndNoProviderPaymentsWillBeGenerated()
        {
            var monthEndCommand = new ProcessProviderMonthEndCommand
            {
                CollectionPeriod = CurrentCollectionPeriod,
                Ukprn = TestSession.Ukprn,
                JobId = TestSession.JobId
            };
            await MessageSession.Send(monthEndCommand);
            var matcher = new ProviderPaymentEventMatcher(CurrentCollectionPeriod, TestSession);
            await WaitForUnExpected(() => matcher.MatchNoPayments(), "Provider Payment event check failure");
        }
        

        [Then(@"no payments will be calculated for following collection periods")]
        public void ThenNoPaymentsWillBeCalculatedForFollowingCollectionPeriods(Table table)
        {
        }
    }
}