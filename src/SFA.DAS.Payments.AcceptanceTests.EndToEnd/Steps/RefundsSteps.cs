using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.Tests.Core.Builders;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class RefundsSteps : EndToEndStepsBase
    {
        public RefundsSteps(FeatureContext context) : base(context)
        {
        }

        [Given("\"(.*)\" previously submitted the following learner details")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsForProvider(string providerId, Table table)
        {
            GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetails(table);
        }

        [Given(@"the provider previously submitted the following learner details")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetails(Table table)
        {
            PreviousIlr = table.CreateSet<Training>().ToList();
            AddTestLearners(PreviousIlr, TestSession.Provider.Ukprn);
        }

        [Given(@"the ""(.*)"" previously submitted the following learner details")]
        public void GivenThePreviouslySubmittedTheFollowingLearnerDetails(string providerIdentifier, Table table)
        {
            var provider = TestSession.GenerateProvider();
            provider.Identifier = providerIdentifier;
            TestSession.Providers.Add(provider);

            var newIlrSubmission = table.CreateSet<Training>().ToList();
            AddTestLearners(newIlrSubmission, provider.Ukprn);

            PreviousIlr.AddRange(newIlrSubmission);
        }

        [Given(@"the following earnings had been generated for the learner")]
        public void GivenTheFollowingEarningsHadBeenGeneratedForTheLearner(Table table)
        {
            PreviousEarnings = CreateEarnings(table, TestSession.Provider.Ukprn);
            // for new style specs where no ILR specified
            if (PreviousIlr == null)
            {
                PreviousIlr = CreateTrainingFromLearners(TestSession.Provider.Ukprn);
            }
        }

        [Given(@"the following earnings had been generated for the learner for ""(.*)""")]
        public void GivenTheFollowingEarningsHadBeenGeneratedForTheLearnerFor(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            var previousProviderEarnings = CreateEarnings(table, provider.Ukprn);
            PreviousEarnings.AddRange(previousProviderEarnings);

            // for new style specs where no ILR specified
            if (PreviousIlr == null || PreviousIlr.All(u => u.Ukprn != provider.Ukprn))
            {
                var providerPreviousIlr = CreateTrainingFromLearners(TestSession.Provider.Ukprn);
                PreviousIlr.AddRange(providerPreviousIlr);
            }
        }

        [Given("the following payments had been generated for \"(.*)\"")]
        public async Task GivenTheFollowingProviderPaymentsHadBeenGenerated(string providerId, Table table)
        {
            await GivenTheFollowingProviderPaymentsHadBeenGenerated(table);
        }

        [Given(@"the following provider payments had been generated")]
        public async Task GivenTheFollowingProviderPaymentsHadBeenGenerated(Table table)
        {
            await GeneratePreviousPayment(table, TestSession.Provider.Ukprn);
        }

        [Given(@"the following ""(.*)"" payments had been generated")]
        public async Task GivenTheFollowingPaymentsHadBeenGenerated(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await GeneratePreviousPayment(table, provider.Ukprn);
        }

        [Given("the Provider now changes the Learner's ULN to \"(.*)\"")]
        public void TheProviderChangesTheLearnersUln(long newUln)
        {
            TestSession.Learner.Uln = newUln;
            CurrentIlr = PreviousIlr;
        }

        [When(@"the amended ILR file is re-submitted for the learners in collection period (.*)")]
        [When(@"the ILR file is submitted for the learners for collection period (.*)")]
        public async Task WhenTheAmendedILRFileIsRe_SubmittedForTheLearnersInCollectionPeriodRCurrentAcademicYear(string collectionPeriodText)
        {
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build();
            if (Context.ContainsKey("current_collection_period") && (CurrentCollectionPeriod.Period != collectionPeriod.Period || CurrentCollectionPeriod.AcademicYear != collectionPeriod.AcademicYear))
            {
                await RequiredPaymentsCacheCleaner.ClearCaches(TestSession);
                await Task.Delay(Config.TimeToPause);
            }

            SetCollectionPeriod(collectionPeriodText);
        }

        [Then(@"only the following provider payments will be recorded")]
        public async Task ThenTheFollowingProviderPaymentsWillBeRecorded(Table table)
        {
            await ValidateRecordedProviderPayments(table, TestSession.Provider);
        }

        [Then(@"only the following ""(.*)"" payments will be recorded")]
        public async Task  ThenOnlyTheFollowingPaymentsWillBeRecorded(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await ValidateRecordedProviderPayments(table, provider);
        }
        
    }
}