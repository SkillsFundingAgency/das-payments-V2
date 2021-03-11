using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using TechTalk.SpecFlow;
using PublishOptions = NServiceBus.PublishOptions;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "Providers Requiring Resubmission")]
    public class ProvidersRequiringResubmissionSteps : EndToEndStepsBase
    {
        public ProvidersRequiringResubmissionSteps(FeatureContext context) : base(context)
        {
        }

        [Given(@"there is no previous submission from provider in current collection period")]
        public void GivenThereIsNoPreviousSubmissionFromProviderInCurrentCollectionPeriod()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"there is a change at approvals side")]
        public async Task WhenThereIsAChangeAtApprovalsSide()
        {
            ScenarioContext.Current.Pending();
            var options = new PublishOptions();
            await MessageSession.Publish<ApprenticeshipUpdated>(m =>
            {
                m.Ukprn = 123;
            }, options);
        }

        [Then(@"new record will be added to the  ProviderRequiringReprocessing table")]
        public void ThenNewRecordWillBeAddedToTheProviderRequiringReprocessingTable()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"there is previous successful/unsuccessful submission from provider in current collection period")]
        public void GivenThereIsPreviousSuccessfulUnsuccessfulSubmissionFromProviderInCurrentCollectionPeriod()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"a provider already exists in ProviderRequiringReprocessing table")]
        public void GivenAProviderAlreadyExistsInProviderRequiringReprocessingTable()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"there is a change at approvals side but no new submission has been made by provider")]
        public void WhenThereIsAChangeAtApprovalsSideButNoNewSubmissionHasBeenMadeByProvider()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"there should not be any change to ProviderRequiringReprocessing table")]
        public void ThenThereShouldNotBeAnyChangeToProviderRequiringReprocessingTable()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"a provider exists in ProviderRequiringReprocessing for current collection period")]
        public void GivenAProviderExistsInProviderRequiringReprocessingForCurrentCollectionPeriod()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"new successful \(appears in latest successful jobs view\) submission is processed from that provider")]
        public void WhenNewSuccessfulAppearsInLatestSuccessfulJobsViewSubmissionIsProcessedFromThatProvider()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"record for provider should be deleted from the ProviderRequiringReprocessing table")]
        public void ThenRecordForProviderShouldBeDeletedFromTheProviderRequiringReprocessingTable()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"new unsuccessful submission is processed from that provider")]
        public void WhenNewUnsuccessfulSubmissionIsProcessedFromThatProvider()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"record for provider should not be deleted from the ProviderRequiringReprocessing table")]
        public void ThenRecordForProviderShouldNotBeDeletedFromTheProviderRequiringReprocessingTable()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
