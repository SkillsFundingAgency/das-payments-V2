using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
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

        [Given(@"the provider is providing trainging for the following learners")]
        public void GivenTheProviderIsProvidingTraingingForTheFollowingLearners(Table table)
        {
            CurrentIlr = table.CreateSet<Training>().ToList();
        }

        [Then(@"the following learner earnings should be generated")]
        public async Task ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            var earnings = table.CreateSet<OnProgrammeEarning>().ToList();
            //var fm36Learners = new List<FM36Learner>();
            //foreach (var training in CurrentIlr)
            //{
            //    var learner = new FM36Learner();
            //    PopulateLearner(learner, training, earnings);
            //    fm36Learners.Add(learner);
            //}

            //await DcHelper.SendIlrSubmission(fm36Learners, TestSession.Ukprn, CollectionYear);


            foreach (var training in CurrentIlr)
            {
                var learner = new FM36Learner();
                PopulateLearner(learner, training, earnings);
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
            await WaitForIt(() => EarningEventMatcher.MatchEarnings(earnings, TestSession.Ukprn, TestSession.Learner.LearnRefNumber, TestSession.JobId), "OnProgrammeEarning event check failure");
        }

        [Then(@"only the following payments will be calculated")]
        public async Task ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = table.CreateSet<Payment>().ToList();
            var matcher = new RequiredPaymentEventMatcher(TestSession,  CurrentCollectionPeriod, expectedPayments);
            await WaitForIt(() => matcher.MatchPayments(), "Required Payment event check failure");
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
    }
}