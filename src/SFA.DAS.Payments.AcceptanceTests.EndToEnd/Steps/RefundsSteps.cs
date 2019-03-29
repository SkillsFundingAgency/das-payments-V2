using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using ESFA.DC.ILR.TestDataGenerator.Models;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
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
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
        
            var newIlrSubmission = table.CreateSet<Training>().ToList();
            AddTestLearners(newIlrSubmission, provider.Ukprn);

            if (PreviousIlr == null) PreviousIlr = new List<Training>();

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
            TestSession.Learner.Uln = newUln;
            CurrentIlr = PreviousIlr;
        }

        [When(@"the amended ILR file is re-submitted for the learners in collection period (.*)")]
        [When(@"the ILR file is submitted for the learners for collection period (.*)")]
        public async Task WhenIlrFileIsSubmittedForTheLearnersInCollectionPeriod(string collectionPeriodText)
        {
            var collectionYear = collectionPeriodText.ToDate().Year;
            var collectionPeriod = new CollectionPeriodBuilder().WithSpecDate(collectionPeriodText).Build().Period;

            await WhenIlrFileIsSubmittedForTheLearnersInCollectionPeriod(collectionPeriodText, TestSession.Provider.Identifier).ConfigureAwait(false);

            if (CurrentIlr != null && CurrentIlr.Any())
            {
                var mapper = Scope.Resolve<IMapper>();
                var mappedrecord = mapper.Map<NonLevyLearnerRequest>(CurrentIlr.First());
                var ilrFile = await GenerateTestIlrFile(mappedrecord);
                
                // currently only support a single ILR file being generated.
                if (ilrFile.Any())
                {
                    await StoreAndPublishIlrFile(learnerRequest: mappedrecord, ilrFileName: ilrFile.First().Key, ilrFile: ilrFile.First().Value, collectionYear: collectionYear, collectionPeriod: collectionPeriod);
                }
            }
        }

        [When(@"the ILR file is submitted for the learners for the collection period (.*) by ""(.*)""")]
        [When(@"the amended ILR file is re-submitted for the learners in the collection period (.*) by ""(.*)""")]
        public async Task WhenIlrFileIsSubmittedForTheLearnersInCollectionPeriod(string collectionPeriodText, string providerIdentifier)
        {
            var provider = TestSession.GetProviderByIdentifier(providerIdentifier);
            await HandleIlrReSubmissionForTheLearners(collectionPeriodText, provider).ConfigureAwait(false);
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
            await WaitForUnexpected(() => matcher.MatchNoPayments(), "Recorded payments check failed").ConfigureAwait(false);
        }        
    }
}