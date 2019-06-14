using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Tests.Core;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class ApprovalsInterfaceSteps : EndToEndStepsBase
    {
        public List<ApprovalsEmployer> Employers
        {
            get => Get<List<ApprovalsEmployer>>();
            set => Set(value);
        }

        public List<ApprovalsApprenticeship> ApprovalsApprenticeships
        {
            get => Get<List<ApprovalsApprenticeship>>();
            set => Set(value);
        }

        public static IMessageSession DasMessageSession { get; set; }

        public ApprovalsInterfaceSteps(FeatureContext context) : base(context)
        {
        }

        [BeforeTestRun(Order = 0)]
        public static void SetUpDasEndpoint()
        {
            var config = new TestsConfiguration();
            var endpointConfig = new EndpointConfiguration(config.AcceptanceTestsEndpointName);
            Builder.RegisterInstance(endpointConfig)
                .Named<EndpointConfiguration>("DasEndpointConfiguration")
                .SingleInstance();
            var conventions = endpointConfig.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());

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
            var endpointConfiguration = Container.ResolveNamed<EndpointConfiguration>("DasEndpointConfiguration");
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(Container));
            DasMessageSession = Endpoint.Start(endpointConfiguration).Result;
        }

        [Given(@"the following employers")]
        public void GivenTheFollowingEmployer(Table employers)
        {
            Employers = employers.CreateSet<ApprovalsEmployer>().ToList();
            Employers.ForEach(employer =>
            {
                employer.AccountId = TestSession.GenerateId();
                Console.WriteLine($"Employer: {employer.ToJson()}");
            });
        }

        [Given(@"the following apprenticeships have been approved")]
        public void GivenTheFollowingApprenticeshipsHaveBeenApproved(Table table)
        {
            ApprovalsApprenticeships = table.CreateSet<ApprovalsApprenticeship>().ToList();
            ApprovalsApprenticeships.ForEach(apprenticeship =>
            {
                apprenticeship.Id = TestSession.GenerateId();
                Console.WriteLine($"Apprenticeship: {apprenticeship.ToJson()}");
            });
        }

        [Given(@"the apprenticeships have the following price episodes")]
        public void GivenTheApprenticeshipsHaveTheFollowingPriceEpisodes(Table table)
        {
            var priceEpisodes = table.CreateSet<ApprovalsApprenticeship.PriceEpisode>();
            foreach (var priceEpisode in priceEpisodes)
            {
                Console.WriteLine($"adding price episode to apprenticeship. Price episode: {priceEpisode.ToJson()}");
                var apprenticeship = ApprovalsApprenticeships.FirstOrDefault(appr => appr.Identifier == priceEpisode.Apprenticeship);
                if (apprenticeship == null)
                    Assert.Fail($"Failed to find the apprenticeship for price episode: {priceEpisode.ToJson()}");
                apprenticeship.PriceEpisodes.Add(priceEpisode);
            }
        }

        [When(@"the Approvals service notifies the Payments service of the apprenticeships")]
        public void WhenTheApprovalsServiceNotifiesPaymentsVOfTheApprenticeships()
        {
            foreach (var approvalsApprenticeship in ApprovalsApprenticeships)
            {
                var employer = Employers.FirstOrDefault(emp => emp.Identifier == approvalsApprenticeship.Employer);
                if (employer == null)
                    Assert.Fail($"Failed to find employer: {approvalsApprenticeship.Employer}");
                var sendingEmployer =
                    Employers.FirstOrDefault(emp => emp.Identifier == approvalsApprenticeship.SendingEmployer);
                var provider = TestSession.GetProviderByIdentifier(approvalsApprenticeship.Provider);
                if (provider == null)
                    Assert.Fail($"Failed to generate provider: {approvalsApprenticeship.Provider}");

                var createdMessage = new CommitmentsV2.Messages.Events.ApprenticeshipCreatedEvent
                {
                    AccountId = employer.AccountId,
                    StartDate = approvalsApprenticeship.StartDate.ToDate(),
                    EndDate = approvalsApprenticeship.EndDate.ToDate(),
                    AccountLegalEntityPublicHashedId = employer.AgreementId,
                    AgreedOn = approvalsApprenticeship.AgreedOnDate.ToDate(),
                    ApprenticeshipId = approvalsApprenticeship.Id,
                    CreatedOn = approvalsApprenticeship.CreatedOnDate.ToDate(),
                    LegalEntityName = employer.Name,
                    ProviderId = provider.Ukprn,
                    TrainingType = ProgrammeType.Standard,
                    TrainingCode = approvalsApprenticeship.StandardCode.ToString(),
                    TransferSenderId = sendingEmployer?.AccountId,
                    Uln = approvalsApprenticeship.Id.ToString(),
                    PriceEpisodes = approvalsApprenticeship.PriceEpisodes.Select(pp => new CommitmentsV2.Messages.Events.PriceEpisode
                    {
                        FromDate = pp.EffectiveFrom.ToDate(),
                        ToDate = pp.EffectiveTo?.ToDate(),
                        Cost = pp.Amount
                    }).ToArray()
                };
                Console.WriteLine($"Sending CreatedApprenticeship message: {createdMessage.ToJson()}");
                DasMessageSession.Send(createdMessage).ConfigureAwait(false);
            }
        }

        [Then(@"the Payments service should record the apprenticeships")]
        public void ThenPaymentsVShouldRecordTheApprenticeships()
        {
            ScenarioContext.Current.Pending();
        }

    }
}