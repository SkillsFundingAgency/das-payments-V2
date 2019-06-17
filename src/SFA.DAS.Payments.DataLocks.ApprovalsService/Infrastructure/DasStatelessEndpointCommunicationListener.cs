using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Infrastructure
{
    public interface IDasStatelessEndpointCommunicationListener : IStatelessEndpointCommunicationListener { }

    public class DasStatelessEndpointCommunicationListener : IDasStatelessEndpointCommunicationListener
    {
        private readonly IConfigurationHelper configHelper;
        private readonly IApplicationConfiguration config;
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IEndpointInstance endpointInstance;

        public DasStatelessEndpointCommunicationListener(IConfigurationHelper configHelper, IApplicationConfiguration config, IPaymentLogger logger, ILifetimeScope lifetimeScope)
        {
            this.configHelper = configHelper ?? throw new ArgumentNullException(nameof(configHelper));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Opening endpoint: {config.EndpointName}");
                if (endpointInstance == null)
                {
                    var endpointConfiguration = CreateEndpointConfiguration();
                    //endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(lifetimeScope));
                    endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
                }
                logger.LogInfo($"Finished opening endpoint listener: {config.EndpointName}");
                return "das endpoint comms listener";
            }
            catch (Exception e)
            {
                logger.LogFatal($"Cannot start the endpoint: '{config.EndpointName}'.  Error: {e.Message}", e);
                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return endpointInstance?.Stop();
        }

        public void Abort()
        {
            CloseAsync(CancellationToken.None);
        }

        private EndpointConfiguration CreateEndpointConfiguration()
        {
            var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions
                .DefiningMessagesAs(t => t.IsAssignableTo<ApprenticeshipCreatedEvent>())
                .DefiningEventsAs(t => t.IsAssignableTo<ApprenticeshipCreatedEvent>());

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(configHelper.GetConnectionString("DASServiceBusConnectionString"))
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);
            //            EndpointConfigurationEvents
            //                .OnConfiguringTransport(transport); //TODO: find AutoFac & NSB way to do this
            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            //          endpointConfiguration.Pipeline.Register(typeof(TelemetryHandlerBehaviour),
            //              "Sends handler timing to telemetry service.");
            //         endpointConfiguration.EnableCallbacks(makesRequests: false);

            if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior),
                "Logs exceptions to the payments logger");
            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton((IPaymentLogger)logger));
            return endpointConfiguration;

        }
    }
}