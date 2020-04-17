using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules
{
    public class MessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessagingLoggerFactory>();
            builder.RegisterType<MessagingLogger>();

            builder.Register((c, p) =>
                             {
                                 var config = c.Resolve<IScheduledJobsConfiguration>();
                                 var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

                                 endpointConfiguration.CustomDiagnosticsWriter(diagnostics =>
                                                                               {
                                                                                   var logger = c.Resolve<MessagingLogger>();
                                                                                   logger.Info(diagnostics);
                                                                                   return Task.CompletedTask;
                                                                               });

                                 var conventions = endpointConfiguration.Conventions();
                                 conventions.DefiningCommandsAs(type => ( type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false ) && (bool) type.Namespace?.Contains(".Messages.Commands"));

                                 endpointConfiguration.DisableFeature<TimeoutManager>();
                                 if (!string.IsNullOrEmpty(config.NServiceBusLicense))
                                 {
                                     var license = WebUtility.HtmlDecode(config.NServiceBusLicense);
                                     endpointConfiguration.License(license);
                                 }

                                 var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                                 transport.ConnectionString(config.ServiceBusConnectionString).Transactions(TransportTransactionMode.ReceiveOnly).RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
                                 transport.PrefetchCount(20);
                                 builder.RegisterInstance(transport).As<TransportExtensions<AzureServiceBusTransport>>().SingleInstance();
                                 endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
                                 endpointConfiguration.EnableInstallers();

                                 return endpointConfiguration;
                             })
                   .As<EndpointConfiguration>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<EndpointInstanceFactory>().As<IEndpointInstanceFactory>().InstancePerLifetimeScope();
        }
    }
}