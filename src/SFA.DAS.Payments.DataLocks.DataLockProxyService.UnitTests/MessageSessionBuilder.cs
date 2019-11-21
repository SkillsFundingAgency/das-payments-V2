using System;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.UnitTests
{
    class MessageSessionBuilder
    {
        public static async Task<IMessageSession> BuildAsync()
        {
            var connectionString = "";
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Please set the connection string in 'MessageSessionBuilder.cs' - don't forget" +
                                    "to remove it when you're done!");
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
