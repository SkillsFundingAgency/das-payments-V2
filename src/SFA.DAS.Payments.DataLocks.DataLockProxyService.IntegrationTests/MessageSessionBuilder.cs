using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NServiceBus;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests
{
    class MessageSessionBuilder
    {
        public static async Task<IMessageSession> BuildAsync()
        {
            var conf = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.development.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = conf["ServiceBusConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Please include an appsettings.development.json file " +
                                    "and include the connection string");
            }

            var builder = new ContainerBuilder();
            
            var configuration = new EndpointConfiguration("sfa-das-payments-datalock-proxy-service-tester");
            builder.RegisterInstance(configuration)
                .SingleInstance();
            var conventions = configuration.Conventions();
            conventions.DefiningMessagesAs(type => type.IsMessage());

            var transportConfig = configuration.UseTransport<AzureServiceBusTransport>();
            
            transportConfig
                .ConnectionString(connectionString)
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
