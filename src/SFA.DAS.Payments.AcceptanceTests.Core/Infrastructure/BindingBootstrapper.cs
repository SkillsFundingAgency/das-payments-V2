using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NServiceBus;
using NServiceBus.Faults;
using NServiceBus.Features;
using SFA.DAS.Payments.Messages.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure
{
    [Binding]
    public class BindingBootstrapper : BindingsBase
    {
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
                .ConnectionString(config.ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly);
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

        [BeforeTestRun(Order = 75)]
        public static async Task ClearQueue()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(Config.ServiceBusConnectionString);
            if (!namespaceManager.QueueExists(Config.AcceptanceTestsEndpointName))
            {
                Console.WriteLine($"'{Config.AcceptanceTestsEndpointName}' not found.");
                return;
            }
            Console.WriteLine($"Now clearing queue: '{Config.AcceptanceTestsEndpointName}'");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var messagingFactory = MessagingFactory.CreateFromConnectionString(Config.ServiceBusConnectionString);
            var receiver = await messagingFactory.CreateMessageReceiverAsync(Config.AcceptanceTestsEndpointName, ReceiveMode.ReceiveAndDelete);
            while (true)
            {
                var messages = await receiver.ReceiveBatchAsync(500, TimeSpan.FromSeconds(1));
                if (!messages.Any())
                {
                    break;
                }
            };
            Console.WriteLine($"Finished purging messages from {Config.AcceptanceTestsEndpointName}. Took: {stopwatch.ElapsedMilliseconds}ms");
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
