using System.Linq;
using Autofac;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Application.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using PublishOptions = NServiceBus.PublishOptions;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "Providers Requiring Resubmission")]
    public class ProvidersRequiringResubmissionSteps : EndToEndStepsBase
    {
        private long ukprn = 93753;

        public ProvidersRequiringResubmissionSteps(FeatureContext context) : base(context)
        {
        }

        [Given(@"there is no previous submission from provider in current collection period")]
        public void GivenThereIsNoPreviousSubmissionFromProviderInCurrentCollectionPeriod()
        {
        }

        [When(@"there is a change at approvals side")]
        public async Task WhenThereIsAChangeAtApprovalsSide()
        {
            var options = new PublishOptions();
            await MessageSession.Publish<ApprenticeshipUpdated>(m =>
            {
                m.Ukprn = ukprn;
            }, options);
        }

        [Then(@"new record will be added to the ProviderRequiringReprocessing table")]
        public void ThenNewRecordWillBeAddedToTheProviderRequiringReprocessingTable()
        {
        }

        [Given(@"there is previous successful/unsuccessful submission from provider in current collection period")]
        public void GivenThereIsPreviousSuccessfulUnsuccessfulSubmissionFromProviderInCurrentCollectionPeriod()
        {
        }

        [Given(@"a provider already exists in ProviderRequiringReprocessing table")]
        public void GivenAProviderAlreadyExistsInProviderRequiringReprocessingTable()
        {
        }

        [When(@"there is a change at approvals side but no new submission has been made by provider")]
        public void WhenThereIsAChangeAtApprovalsSideButNoNewSubmissionHasBeenMadeByProvider()
        {
        }

        [Then(@"there should not be any change to ProviderRequiringReprocessing table")]
        public void ThenThereShouldNotBeAnyChangeToProviderRequiringReprocessingTable()
        {
        }

        [Given(@"a provider exists in ProviderRequiringReprocessing for current collection period")]
        public void GivenAProviderExistsInProviderRequiringReprocessingForCurrentCollectionPeriod()
        {
        }

        [When(@"new successful \(appears in latest successful jobs view\) submission is processed from that provider")]
        public async Task WhenNewSuccessfulAppearsInLatestSuccessfulJobsViewSubmissionIsProcessedFromThatProvider()
        {
            var options = new PublishOptions();
            await MessageSession.Publish<SubmissionJobSucceeded>(m =>
            {
                m.Ukprn = ukprn;
            }, options);
        }

        [Then(@"record for provider should be deleted from the ProviderRequiringReprocessing table")]
        public async Task ThenRecordForProviderShouldBeDeletedFromTheProviderRequiringReprocessingTable()
        {
            var dataContext = Scope.Resolve<PeriodEndDataContext>();
            await WaitForIt(async () =>
            {
                return !(await dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == ukprn));
            }, $"Failed to find provider with matching ukprn: {ukprn} in ProviderRequiringReprocessing table ");

        }

        [When(@"new unsuccessful submission is processed from that provider")]
        public async Task WhenNewUnsuccessfulSubmissionIsProcessedFromThatProvider()
        {
            var options = new PublishOptions();
            await MessageSession.Publish<SubmissionJobFailed>(m =>
            {
                m.Ukprn = ukprn;
            }, options);
        }

        [Then(@"record for provider should not be deleted from the ProviderRequiringReprocessing table")]
        public async Task ThenRecordForProviderShouldNotBeDeletedFromTheProviderRequiringReprocessingTable()
        {
            var dataContext = Scope.Resolve<PeriodEndDataContext>();
            await WaitForIt(() =>
            {
                return dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == ukprn);
            }, $"Failed to find provider with matching ukprn: {ukprn} in ProviderRequiringReprocessing table ");
        }
    }
}