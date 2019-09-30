using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing;
using SFA.DAS.Payments.Monitoring.Jobs.Data;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    internal static class Program
    {
        private static ReliableStateManagerProvider stateManager;

        private static async Task Main()
        {
            try
            {
                using (CreateContainerForStatefulService<JobService>())
                {
                    await Task.Delay(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        //private static async Task StartEndpoint()
        //{
        //    IApplicationConfiguration config = ApplicationConfiguration();

        //    var endpointConfiguration = ConfigureEndpoint(config);
        //    ConfigureServices(endpointConfiguration, config);

        //    var endpointInstance = await Endpoint.Start(endpointConfiguration);
        //}

        private static IDisposable CreateContainerForStatefulService<TStatefulService>() where TStatefulService : StatefulService
        {
            var builder = ContainerFactory.CreateBuilder();

            builder.Register(_ => ConfigureEndpoint()).SingleInstance();

            //builder.RegisterType<StateManagerUnitOfWork>().As<IManageUnitsOfWork>().InstancePerLifetimeScope();

            builder.RegisterStatefulService<TStatefulService>(typeof(TStatefulService).Namespace + "Type")
                .OnActivating(e =>
                {
                    stateManager.Current = e.Instance.StateManager;
                });
            var container = ContainerFactory.CreateContainer(builder);

            stateManager = (ReliableStateManagerProvider)container.Resolve<IReliableStateManagerProvider>();

            return container;
        }

        private static ListenerFactory ConfigureEndpoint()
        {
            var config = ApplicationConfiguration();

            Func<EndpointConfiguration> configureEndpoint = () =>
            {
                var endpointConfiguration = ConfigureEndpoint(config);
                ConfigureServices(endpointConfiguration, config);

                return endpointConfiguration;
            };

            return new ListenerFactory(configureEndpoint, config);
        }

        private static EndpointConfiguration ConfigureEndpoint(IApplicationConfiguration config)
        {
            var endpointName = new EndpointName(config.EndpointName);
            EndpointConfigurationEvents.OnConfiguringEndpoint(endpointName);
            var endpointConfiguration = new EndpointConfiguration(endpointName.Name);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            conventions.DefiningCommandsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false)
                && ((type.Namespace?.Contains(".Messages.Events") ?? false) || (type.Namespace?.Contains(".Messages.Core") ?? false)));

            //var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            //persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();

            if (!string.IsNullOrEmpty(config.NServiceBusLicense))
            {
                var license = WebUtility.HtmlDecode(config.NServiceBusLicense);
                endpointConfiguration.License(license);
            }

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(config.ServiceBusConnectionString)
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            //endpointConfiguration.Pipeline.Register(typeof(TelemetryHandlerBehaviour), "Sends handler timing to telemetry service.");
            //endpointConfiguration.EnableCallbacks(makesRequests: false);
            //endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior), "Logs exceptions to the payments logger");

            // values 2 and 4 work best, as tx is serializable it makes no sense to allow many concurrent tasks
            var perReceiverConcurrency = 2;
            // increase number of receivers as much as bandwidth allows
            var numberOfReceivers = 16;

            endpointConfiguration.LimitMessageProcessingConcurrencyTo(numberOfReceivers * perReceiverConcurrency);
            transport.PrefetchMultiplier(20);

            return endpointConfiguration;
        }

        private static IApplicationConfiguration ApplicationConfiguration()
        {
            var configHelper = new ServiceFabricConfigurationHelper();

            return new ApplicationConfiguration
            {
                EndpointName = configHelper.GetSetting("EndpointName"),
                StorageConnectionString = configHelper.GetConnectionString("StorageConnectionString"),
                ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString"),
                FailedMessagesQueue = configHelper.GetSetting("FailedMessagesQueue"),
                ProcessMessageSequentially = false,
                NServiceBusLicense = configHelper.GetSetting("DasNServiceBusLicenseKey")
            };
        }

        private static void ConfigureServices(EndpointConfiguration endpointConfiguration, IApplicationConfiguration config)
        {
            var configHelper = new ServiceFabricConfigurationHelper();

            endpointConfiguration.RegisterComponents(configureComponents =>
            {
                configureComponents.ConfigureComponent<IExecutionContext>(s =>
                    new ESFA.DC.Logging.ExecutionContext(), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<ILoggerConfigurationBuilder>(s =>
                    new LoggerConfigurationBuilder(), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent(s =>
                    new PaymentsSerilogLoggerFactory(s.Build<ILoggerConfigurationBuilder>()), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<ISerilogLoggerFactory>(s => s.Build<PaymentsSerilogLoggerFactory>(), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<IPaymentLogger>(s =>
                    new PaymentLogger(LoggerSettings(), s.Build<IExecutionContext>(), s.Build<PaymentsSerilogLoggerFactory>()), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<IJobsDataContext>(s =>
                    new JobsDataContext(config.StorageConnectionString)
                    , DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<IReliableStateManagerTransactionProvider>(s =>
                    new ReliableStateManagerTransactionProvider(), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<IReliableStateManagerProvider>(() => stateManager, DependencyLifecycle.SingleInstance);

                configureComponents.ConfigureComponent<IJobStorageService>(s =>
                    new JobStorageService(
                        s.Build<IReliableStateManagerProvider>(),
                        s.Build<IReliableStateManagerTransactionProvider>(),
                        s.Build<IJobsDataContext>(),
                        s.Build<IPaymentLogger>())
                    , DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<IJobMessageService>(s =>
                    new JobMessageService(
                        s.Build<IJobStorageService>(),
                        s.Build<IPaymentLogger>())
                    , DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent(s =>
                    new TelemetryConfiguration(configHelper.GetSetting("ApplicationInsightsInstrumentationKey")), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent(s =>
                    new TelemetryClient(s.Build<TelemetryConfiguration>()), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<ITelemetry>(s =>
                    new ApplicationInsightsTelemetry(s.Build<TelemetryClient>()), DependencyLifecycle.InstancePerUnitOfWork);

                configureComponents.ConfigureComponent<IEarningsJobService>(s =>
                    new EarningsJobService(
                        s.Build<IPaymentLogger>(),
                        s.Build<IJobStorageService>(),
                        s.Build<ITelemetry>())
                    , DependencyLifecycle.InstancePerUnitOfWork);
            });
        }

        private static ApplicationLoggerSettings LoggerSettings()
        {
            var configHelper = new ServiceFabricConfigurationHelper();

            if (!Enum.TryParse(configHelper.GetSettingOrDefault("LogLevel", "Information"), out LogLevel logLevel))
                logLevel = LogLevel.Information;

            if (!Enum.TryParse(configHelper.GetSettingOrDefault("ConsoleLogLevel", "Information"), out LogLevel consoleLogLevel))
                consoleLogLevel = logLevel;

            var loggerSettings = new ApplicationLoggerSettings
            {
                ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                        {
                            new MsSqlServerApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = logLevel,
                                ConnectionString = configHelper.GetConnectionString("LoggingConnectionString"),
                            },

                            new ConsoleApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = consoleLogLevel
                            },
                        },
            };
            return loggerSettings;
        }
    }

    public class ListenerFactory
    {
        private readonly IApplicationConfiguration config;
        private readonly Func<EndpointConfiguration> endpointConfiguration;

        public ListenerFactory(Func<EndpointConfiguration> endpointConfiguration, IApplicationConfiguration config)
        {
            this.config = config;
            this.endpointConfiguration = endpointConfiguration;
        }

        public IStatefulEndpointCommunicationListener Build()
        {
            var factory = new EndpointInstanceFactory(endpointConfiguration());
            return new StatefulEndpointCommunicationListener(factory, config);
        }
    }
}
