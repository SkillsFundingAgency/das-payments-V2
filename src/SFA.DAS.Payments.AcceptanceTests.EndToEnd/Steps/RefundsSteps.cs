using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Handlers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class RefundsSteps : EndToEndStepsBase
    {
        private readonly FeatureNumber featureNumber;
        private readonly ScenarioInfo scenarioInfo;

        public RefundsSteps(FeatureContext context, FeatureNumber featureNumber, ScenarioInfo scenarioInfo) : base(context)
        {
            this.featureNumber = featureNumber;
            this.scenarioInfo = scenarioInfo;
        }

        [Given("\"(.*)\" previously submitted the following learner details")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetailsForProvider(string providerId, Table table)
        {
            GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetails(table);
        }

        [Given(@"the provider previously submitted the following learner details")]
        public void GivenTheProviderPreviouslySubmittedTheFollowingLearnerDetails(Table table)
        {
            AddTestLearners(table);
        }

        [Given(@"the ""(.*)"" previously submitted the following learner details")]
        public void GivenThePreviouslySubmittedTheFollowingLearnerDetails(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);

            var newIlrSubmission = table.CreateSet<Training>().ToList();
            AddTestLearners(newIlrSubmission, provider.Ukprn);

            if (PreviousIlr == null) PreviousIlr = new List<Training>();

            PreviousIlr.AddRange(newIlrSubmission);
        }

        [Given(@"the following earnings had been generated for the learner")]
        public void GivenTheFollowingEarningsHadBeenGeneratedForTheLearner(Table table)
        {
            CreatePreviousEarningsAndTraining(table);
        }


        [Given(@"the following earnings had been generated for the learner for ""(.*)""")]
        public void GivenTheFollowingEarningsHadBeenGeneratedForTheLearnerFor(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            var previousProviderEarnings = CreateEarnings(table, provider.Ukprn);

            if(PreviousEarnings == null) PreviousEarnings = new List<Earning>();

            PreviousEarnings.AddRange(previousProviderEarnings);

            // for new style specs where no ILR specified
            if (PreviousIlr == null || PreviousIlr.All(u => u.Ukprn != provider.Ukprn))
            {
                var providerPreviousIlr = CreateTrainingFromLearners(provider.Ukprn);
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
            TestSession.GetLearner(TestSession.Provider.Ukprn, null).Uln = newUln;
            CurrentIlr = PreviousIlr;
            CurrentIlr.ForEach(x => x.Uln = newUln);
        }

        [When(@"the amended ILR file is re-submitted")]
        public async Task WhenTheAmendedILRFileIsRe_Submitted()
        {
            var amendedLearnerDetails = new Table("Start Date","Planned Duration","Total Training Price","Total Training Price Effective Date","Total Assessment Price","Total Assessment Price Effective Date","Actual Duration","Completion Status","SFA Contribution Percentage","Contract Type","Aim Sequence Number","Aim Reference","Framework Code","Pathway Code","Programme Type","Funding Line Type");
            amendedLearnerDetails.AddRow("start of academic year","12 months","10","Aug/Current Academic Year","0","Aug/Current Academic Year","","continuing","90%","Act2","1","ZPROG001","593","1","20","19+ Apprenticeship Non - Levy Contract(procured)");
            AddNewIlr(amendedLearnerDetails, TestSession.Ukprn);


            var priceEdpisodes = new Table("Price Episode Id","Total Training Price","Total Training Price Effective Date","Total Assessment Price","Total Assessment Price Effective Date","Residual Training Price","Residual Training Price Effective Date","Residual Assessment Price","Residual Assessment Price Effective Date","SFA Contribution Percentage","Contract Type","Aim Sequence Number");
            priceEdpisodes.AddRow("pe-1","10","06/Aug/Current Academic Year","0","06/Aug/Current Academic Year","0","","0","","90%","Act2","1");
            AddPriceDetails(priceEdpisodes);

            await WhenIlrFileIsSubmittedForTheLearnersInCollectionPeriod("R01/Current Academic Year").ConfigureAwait(false);
            //await GeneratedAndValidateEarnings(table, TestSession.Provider).ConfigureAwait(false);
            await GenerateEarnings(TestSession.Provider).ConfigureAwait(false);
        }

        [When(@"the amended ILR file is re-submitted for the learners in collection period (.*)")]
        [When(@"the ILR file is submitted for the learners for collection period (.*)")]
        [Given(@"the ILR file is submitted for the learners for collection period (.*)")]
        public async Task WhenIlrFileIsSubmittedForTheLearnersInCollectionPeriod(string collectionPeriodText)
        {
            Task ClearCache() => HandleIlrReSubmissionForTheLearners(collectionPeriodText, TestSession.Provider);
            await Scope.Resolve<IIlrService>().PublishLearnerRequest(CurrentIlr, TestSession.Learners, collectionPeriodText, featureNumber.Extract(), ClearCache);
        }

        [When(@"the ILR file is submitted for the learners for the collection period (.*) by ""(.*)""")]
        [When(@"the amended ILR file is re-submitted for the learners in the collection period (.*) by ""(.*)""")]
        public async Task WhenIlrFileIsSubmittedForTheLearnersInCollectionPeriod(string collectionPeriodText, string providerIdentifier)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await HandleIlrReSubmissionForTheLearners(collectionPeriodText, provider).ConfigureAwait(false);
        }

        [Then(@"(.*) required payments are generated")]
        public async Task RequiredPaymentsAreGenerated(int count)
        {
            await WaitForIt(() =>
                {
                    return RequiredPaymentEventHandler.ReceivedEvents
                        .Count(e => e.Ukprn == TestSession.Provider.Ukprn) == count;
                },
                $"Failed to find {count} required payments");
        }

        [Then(@"levy month end is ran")]
        public async Task ThenLevyMonthEndIsRan()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await SendLevyMonthEnd();
        }


        [Then(@"only the following provider payments will be recorded")]
        public async Task ThenTheFollowingProviderPaymentsWillBeRecorded(Table table)
        {
            await ValidateRecordedProviderPayments(table, TestSession.Provider);
        }

        [Then(@"only the following ""(.*)"" payments will be recorded")]
        public async Task ThenOnlyTheFollowingPaymentsWillBeRecorded(string providerIdentifier, Table table)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await ValidateRecordedProviderPayments(table, provider);
        }

        [Then(@"no provider payments will be recorded")]
        public async Task ThenNoProviderPaymentsWillBeRecorded()
        {
            await ThenNoProviderPaymentsWillBeRecorded(TestSession.Provider.Identifier).ConfigureAwait(false);
        }

        [Then(@"no ""(.*)"" payments will be recorded")]
        public async Task ThenNoProviderPaymentsWillBeRecorded(string providerIdentifier)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            var matcher = new ProviderPaymentModelMatcher(provider, DataContext, TestSession, CurrentCollectionPeriod);
            await WaitForUnexpected(() => matcher.MatchUnexpectedEvents(), "Recorded payments check failed").ConfigureAwait(false);
        }
    }
}