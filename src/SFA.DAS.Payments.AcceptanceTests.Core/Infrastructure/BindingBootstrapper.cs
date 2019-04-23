using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.TestDataGenerator.Api;
using ESFA.DC.ILR.TestDataGenerator.Api.StorageService;
using ESFA.DC.ILR.TestDataGenerator.Interfaces;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NServiceBus;
using NServiceBus.Features;
using Polly;
using Polly.Registry;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
using SFA.DAS.Payments.AcceptanceTests.Services.Configuration;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
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
            Builder.RegisterType<DcHelper>().SingleInstance();
            Builder.RegisterType<EarningsJobClient>()
                .As<IEarningsJobClient>()
                .InstancePerLifetimeScope();

            Builder.RegisterType<AzureStorageServiceConfig>().As<IAzureStorageKeyValuePersistenceServiceConfig>().InstancePerLifetimeScope();
            Builder.RegisterType<UkprnService>().As<IUkprnService>().InstancePerLifetimeScope();
            Builder.RegisterType<AzureStorageKeyValuePersistenceService>().As<IStreamableKeyValuePersistenceService>().InstancePerLifetimeScope();
            Builder.RegisterType<StorageService>().As<IStorageService>().InstancePerLifetimeScope();
            Builder.RegisterType<TdgService>().As<ITdgService>().InstancePerLifetimeScope();
            Builder.Register(c => new TestSession(c.Resolve<IUkprnService>())).InstancePerLifetimeScope();

            if (config.ValidateDcAndDasServices)
            {
                var ukprnDbOptions = new DbContextOptionsBuilder<UkprnService>()
                    .UseSqlServer(config.PaymentsConnectionString)
                    .Options;

                Builder.RegisterInstance(ukprnDbOptions);
                Builder.RegisterType<IlrDcService>().As<IIlrService>().InstancePerLifetimeScope();
                Builder.RegisterType<UkprnService>().As<IUkprnService>().InstancePerLifetimeScope();
                Builder.RegisterType<DcHelper>().As<IDcHelper>().InstancePerLifetimeScope();
            }
            else
            {
                Builder.RegisterType<IlrNullService>().As<IIlrService>().InstancePerLifetimeScope();
                Builder.RegisterType<RandomUkprnService>().As<IUkprnService>().InstancePerLifetimeScope();
                Builder.RegisterType<DcNullHelper>().As<IDcHelper>().InstancePerLifetimeScope();
            }

            Builder.Register(context =>
                {
                    var registry = new PolicyRegistry();
                    registry.Add(
                        "HttpRetryPolicy",
                        Policy.Handle<HttpRequestException>()
                            .WaitAndRetryAsync(
                                3, // number of retries
                                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // exponential backoff
                                (exception, timeSpan, retryCount, executionContext) =>
                                {
                                    // add logging
                                }));
                    return registry;
                }).As<IReadOnlyPolicyRegistry<string>>()
                .SingleInstance();

            Builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();

            Builder.RegisterType<BespokeHttpClient>().As<IBespokeHttpClient>().InstancePerLifetimeScope();

            Builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();

            Builder.RegisterType<CollectionManagementService>().As<ICollectionManagementService>().InstancePerLifetimeScope();

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
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .Queues()
                .DefaultMessageTimeToLive(config.DefaultMessageTimeToLive);

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
            }

            var queueDescription = namespaceManager.GetQueue(Config.AcceptanceTestsEndpointName);
            if (queueDescription.DefaultMessageTimeToLive != Config.DefaultMessageTimeToLive)
            {
                queueDescription.DefaultMessageTimeToLive = Config.DefaultMessageTimeToLive;
                namespaceManager.UpdateQueue(queueDescription);
            }

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
