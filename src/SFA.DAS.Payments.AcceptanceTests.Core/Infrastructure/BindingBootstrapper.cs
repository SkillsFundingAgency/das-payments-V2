using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Messages.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    [Binding]
    public class BindingBootstrapper : StepsBase
    {
        public BindingBootstrapper(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        public static EndpointConfiguration EndpointConfiguration { get; private set; }

        [BeforeTestRun(Order = -1)]
        public static void TestRunSetUp()
        {
            var config = new TestsConfiguration();
            Builder = new ContainerBuilder();
            Builder.RegisterType<TestsConfiguration>().SingleInstance();
            EndpointConfiguration = new EndpointConfiguration(config.AcceptanceTestsEndpointName);
            Builder.RegisterInstance(EndpointConfiguration)
                .SingleInstance();
            var conventions = EndpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());

            EndpointConfiguration.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(config.StorageConnectionString);
            EndpointConfiguration.DisableFeature<TimeoutManager>();
            var transportConfig = EndpointConfiguration.UseTransport<AzureServiceBusTransport>();
            Builder.RegisterInstance(transportConfig)
                .As<TransportExtensions<AzureServiceBusTransport>>()
                .SingleInstance();
            transportConfig
                .UseForwardingTopology()
                .ConnectionString(config.ServiceBusConnectionString);
            var sanitization = transportConfig.Sanitization();
            var strategy = sanitization.UseStrategy<ValidateAndHashIfNeeded>();
            strategy.RuleNameSanitization(
                ruleNameSanitizer: ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            EndpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            EndpointConfiguration.EnableInstallers();
        }

        [BeforeTestRun(Order = 50)]
        public static void CreateContainer()
        {
            Container = Builder.Build();
        }

        [BeforeTestRun(Order = 99)]
        public static void StartBus()
        {
            var endpointConfiguration = Container.Resolve<EndpointConfiguration>();
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(Container));
            MessageSession = Endpoint.Start(endpointConfiguration).Result;
        }
    }
}
