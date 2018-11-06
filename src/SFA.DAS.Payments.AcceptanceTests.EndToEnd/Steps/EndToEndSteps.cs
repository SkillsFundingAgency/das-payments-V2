using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            SfaContributionPercentage = CurrentIlr[0].SfaContributionPercentage;
        }


        [When(@"the ILR file is submitted for the learners for collection period R(.*)/Current Academic Year")]
        public async Task WhenTheILRFileIsSubmittedForTheLearnersForCollectionPeriodRCurrentAcademicYear(int period)
        {
            SetCurrentCollectionYear();
            CollectionPeriod = (byte) period;
            var fm36Learners = new List<FM36Learner>();
            foreach (var training in CurrentIlr)
            {
                var learner = new FM36Learner();
                PopulateLearner(learner, training);
                fm36Learners.Add(learner);
            }

            await DcHelper.SendIlrSubmission(fm36Learners, TestSession.Ukprn, CollectionYear);
        }


        [Then(@"the following learner earnings should be generated")]
        public void ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            var expectedEarnings = table.CreateSet<OnProgrammeEarning>().ToList();
            WaitForIt(() => EarningEventMatcher.MatchEarnings(expectedEarnings, TestSession.Ukprn), "OnProgrammeEarning event check failure");
        }

        [Then(@"the following payments will be calculated")]
        public void ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = table.CreateSet<Payment>().ToList();
            WaitForIt(() => PaymentEventMatcher.MatchPayments(expectedPayments, TestSession.Ukprn), "Payment event check failure");
        }

        [Then(@"at month end the following provider payments will be generated")]
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
            WaitForIt(() => ProviderPaymentEventMatcher.MatchPayments(expectedPayments, TestSession.Ukprn), "ProviderPayment event check failure");
        }
    }
}
