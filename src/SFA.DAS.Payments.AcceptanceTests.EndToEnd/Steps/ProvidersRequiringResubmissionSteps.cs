using Autofac;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.PeriodEnd.Data;
using SFA.DAS.Payments.PeriodEnd.Model;
using TechTalk.SpecFlow;
using PublishOptions = NServiceBus.PublishOptions;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "Providers Requiring Resubmission")]
    public class ProvidersRequiringResubmissionSteps : EndToEndStepsBase
    {
        private PeriodEndDataContext dataContext;

        [BeforeScenario]
        public async Task Setup()
        {
            TestSession.RegenerateUkprn();
            dataContext = Scope.Resolve<PeriodEndDataContext>();
            var record = await dataContext.ProvidersRequiringReprocessing
                .FirstOrDefaultAsync(x => x.Ukprn == TestSession.Ukprn);
            if (record != null)
            {
                dataContext.ProvidersRequiringReprocessing.Remove(record);
                await dataContext.SaveChanges();
            }
        }
        
        public ProvidersRequiringResubmissionSteps(FeatureContext context) : base(context)
        {
        }

        [Given(@"the provider has made a submission in the current collection period")]
        public void GivenThereIsPreviousSuccessfulUnsuccessfulSubmissionFromProviderInCurrentCollectionPeriod()
        {
        }

        [Given(@"the provider has made no submissions in the current collection period")]
        [Given(@"there have been no new submissions made by the provider")]
        public void GivenThereIsNoPreviousSubmissionFromProviderInCurrentCollectionPeriod()
        {
        }

        [When(@"there is a change to the apprenticeship details for one of the provider's learners")]
        public async Task WhenThereIsAChangeAtApprovalsSide()
        {
            var options = new PublishOptions();
            await MessageSession.Publish<ApprenticeshipUpdated>(m =>
            {
                m.Ukprn = TestSession.Ukprn;
            }, options);
        }

        [Then(@"new record will be added to the ProviderRequiringReprocessing table")]
        public async Task ThenNewRecordWillBeAddedToTheProviderRequiringReprocessingTable()
        {
            await WaitForIt(() =>
            {
                return dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == TestSession.Ukprn);
            }, $"Failed to find provider with matching ukprn: {TestSession.Ukprn} in ProviderRequiringReprocessing table ");
        }

        [Then(@"there should not be any change to ProviderRequiringReprocessing table")]
        public async Task ThenThereShouldNotBeAnyChangeToProviderRequiringReprocessingTable()
        {
            await WaitForIt(() =>
            {
                return dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == TestSession.Ukprn);
            }, $"Failed to find provider with matching ukprn: {TestSession.Ukprn} in ProviderRequiringReprocessing table ");
        }

        [Given(@"a provider already exists in ProviderRequiringReprocessing table")]
        public async Task GivenAProviderExistsInProviderRequiringReprocessingForCurrentCollectionPeriod()
        {
            
            dataContext.ProvidersRequiringReprocessing.Add(new ProviderRequiringReprocessingEntity
            {
                Ukprn = TestSession.Ukprn,
            });
            await dataContext.SaveChanges();
        }

        [When(@"new successful \(appears in latest successful jobs view\) submission is processed from that provider")]
        public async Task WhenNewSuccessfulAppearsInLatestSuccessfulJobsViewSubmissionIsProcessedFromThatProvider()
        {
            var options = new PublishOptions();
            await MessageSession.Publish<SubmissionJobSucceeded>(m =>
            {
                m.Ukprn = TestSession.Ukprn;
            }, options);
        }

        [Then(@"record for provider should be deleted from the ProviderRequiringReprocessing table")]
        public async Task ThenRecordForProviderShouldBeDeletedFromTheProviderRequiringReprocessingTable()
        {
            await WaitForIt(async () =>
            {
                return !(await dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == TestSession.Ukprn));
            }, $"Failed to find provider with matching ukprn: {TestSession.Ukprn} in ProviderRequiringReprocessing table ");

        }

        [When(@"new unsuccessful submission is processed from that provider")]
        public async Task WhenNewUnsuccessfulSubmissionIsProcessedFromThatProvider()
        {
            var options = new PublishOptions();
            await MessageSession.Publish<SubmissionJobFailed>(m =>
            {
                m.Ukprn = TestSession.Ukprn;
            }, options);
        }

        [Then(@"record for provider should not be deleted from the ProviderRequiringReprocessing table")]
        public async Task ThenRecordForProviderShouldNotBeDeletedFromTheProviderRequiringReprocessingTable()
        {
            await WaitForIt(() =>
            {
                return dataContext.ProvidersRequiringReprocessing.AnyAsync(x => x.Ukprn == TestSession.Ukprn);
            }, $"Failed to find provider with matching ukprn: {TestSession.Ukprn} in ProviderRequiringReprocessing table ");
        }
    }
}