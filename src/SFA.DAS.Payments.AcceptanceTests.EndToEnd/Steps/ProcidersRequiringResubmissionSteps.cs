using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class ProcidersRequiringResubmissionSteps : EndToEndStepsBase
    {
        private IMessageSession messageSession;

        [BeforeTestRun(Order = 0)]
        public static void SetUpDasEndpoint()
        {
            var config = new TestsConfiguration();
            var endpointConfig = new EndpointConfiguration(config.AcceptanceTestsEndpointName);
            dasEndpointConfiguration = endpointConfig;
            Builder.RegisterInstance(endpointConfig)
                .Named<EndpointConfiguration>("DasEndpointConfiguration")
                .SingleInstance();
            var conventions = endpointConfig.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());
            conventions
                .DefiningCommandsAs(t => t.IsInNamespace("SFA.DAS.CommitmentsV2.Messages.Events"));

            endpointConfig.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(config.StorageConnectionString);
            endpointConfig.DisableFeature<TimeoutManager>();

            var transportConfig = endpointConfig.UseTransport<AzureServiceBusTransport>();
            Builder.RegisterInstance(transportConfig)
                .Named<TransportExtensions<AzureServiceBusTransport>>("DasTransportConfig")
                .SingleInstance();

            transportConfig
                .UseForwardingTopology()
                .ConnectionString(config.DasServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .Queues()
                .DefaultMessageTimeToLive(config.DefaultMessageTimeToLive);
            var routing = transportConfig.Routing();
            //routing.RouteToEndpoint(typeof(CommitmentsV2.Messages.Events.ApprenticeshipCreatedEvent).Assembly, EndpointNames.PeriodEnd);

            var sanitization = transportConfig.Sanitization();
            var strategy = sanitization.UseStrategy<ValidateAndHashIfNeeded>();
            strategy.RuleNameSanitization(
                ruleNameSanitizer: ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            endpointConfig.UseSerialization<NewtonsoftSerializer>();
            endpointConfig.EnableInstallers();
        }

        [BeforeTestRun(Order = 100)]
        public static void StartBus()
        {
            messageSession = Endpoint.Start(dasEndpointConfiguration).Result;
        }

        [Given(@"there is no previous submission from provider in current collection period")]
        public void GivenThereIsNoPreviousSubmissionFromProviderInCurrentCollectionPeriod()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"there is a change at approvals side")]
        public void WhenThereIsAChangeAtApprovalsSide()
        {
            ScenarioContext.Current.Pending();
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

        [Then(@"new record will be added to the ProviderRequiringReprocessing table")]
        public void ThenNewRecordWillBeAddedToTheProviderRequiringReprocessingTable()
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
