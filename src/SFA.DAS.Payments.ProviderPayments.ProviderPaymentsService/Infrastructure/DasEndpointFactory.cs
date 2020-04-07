using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Infrastructure
{
    public interface IDasEndpointFactory
    {
        Task<IEndpointInstance> GetEndpointInstanceAsync();
    }
    
    public class DasEndpointFactory : IDasEndpointFactory
    {
        private readonly IConfigurationHelper configHelper;
        private readonly IApplicationConfiguration config;
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IEndpointInstance endpointInstance;

        public DasEndpointFactory(IConfigurationHelper configHelper, IApplicationConfiguration config, IPaymentLogger logger, ILifetimeScope lifetimeScope)
        {
            this.configHelper = configHelper ?? throw new ArgumentNullException(nameof(configHelper));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public async Task<IEndpointInstance> GetEndpointInstanceAsync()
        {
            try
            {
                if (endpointInstance != null)
                    return endpointInstance;

                logger.LogDebug($"Opening endpoint: {config.EndpointName}");

                var endpointConfiguration = CreateEndpointConfiguration();

                endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            
                logger.LogInfo($"Finished opening endpoint listener: {config.EndpointName}");

                return endpointInstance;
            }
            catch (Exception e)
            {
                logger.LogFatal($"Cannot start the endpoint: '{config.EndpointName}'.  Error: {e.Message}", e);
                throw;
            }
        }
        
        private EndpointConfiguration CreateEndpointConfiguration()
        {
            var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions
                .DefiningEventsAs(t =>
                    t.IsAssignableTo<RecordedAct1CompletionPayment>()
                );
                
            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(configHelper.GetConnectionString("DASServiceBusConnectionString"))
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);

            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();

            if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior), "Logs exceptions to the payments logger");
            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(logger));
            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(lifetimeScope.Resolve<IContainerScopeFactory>()));
            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(lifetimeScope.Resolve<IEndpointInstanceFactory>()));
            
            return endpointConfiguration;
        }
    }
}