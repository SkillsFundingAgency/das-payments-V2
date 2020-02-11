using System;
using System.Linq;
using System.Net;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var endpointName = new EndpointName(Environment.GetEnvironmentVariable("EndpointName", EnvironmentVariableTarget.Process));
                var endpointConfiguration = new EndpointConfiguration(endpointName.Name);

                var conventions = endpointConfiguration.Conventions();
                conventions.DefiningCommandsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));

                /* if (!string.IsNullOrEmpty(config.NServiceBusLicense))
                {
                    var license = WebUtility.HtmlDecode(config.NServiceBusLicense);
                    endpointConfiguration.License(license);
                } */

                var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                transport
                    .ConnectionString(Environment.GetEnvironmentVariable("ServiceBusConnectionString", EnvironmentVariableTarget.Process))
                    .Transactions(TransportTransactionMode.ReceiveOnly)
                    .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
                transport.PrefetchCount(20);
                builder.RegisterInstance(transport)
                    .As<TransportExtensions<AzureServiceBusTransport>>()
                    .SingleInstance();
                endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                endpointConfiguration.EnableInstallers();
                
                return endpointConfiguration;
            })
            .As<EndpointConfiguration>()
            .SingleInstance();

            builder.RegisterType<EndpointInstanceFactory>()
                .As<IEndpointInstanceFactory>()
                .SingleInstance();
        }
    }

    public class EndpointName
    {
        public EndpointName(string endpointName)
        {
            Name = endpointName;
        }

        public string Name { get; set; }
    }
}