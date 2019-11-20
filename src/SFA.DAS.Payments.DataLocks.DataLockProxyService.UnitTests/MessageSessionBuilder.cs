using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.UnitTests
{
    class MessageSessionBuilder
    {
        public static async Task<IMessageSession> BuildAsync()
        {
            var config = new TestsConfiguration();
            var builder = new ContainerBuilder();
            builder.RegisterType<TestsConfiguration>().SingleInstance();

            var configuration = new EndpointConfiguration(config.AcceptanceTestsEndpointName);
            builder.RegisterInstance(configuration)
                .SingleInstance();
            var conventions = configuration.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());

            configuration.UsePersistence<AzureStoragePersistence>()
                .ConnectionString(config.StorageConnectionString);
            configuration.DisableFeature<TimeoutManager>();

            var transportConfig = configuration.UseTransport<AzureServiceBusTransport>();
            builder.RegisterInstance(transportConfig)
                .As<TransportExtensions<AzureServiceBusTransport>>()
                .SingleInstance();

            transportConfig
                .ConnectionString(config.ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly);
            
            configuration.UseSerialization<NewtonsoftSerializer>();
            configuration.EnableInstallers();

            var container = builder.Build();
            configuration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            var messageSession = await Endpoint.Start(configuration);

            return messageSession;
        }
    }
}
