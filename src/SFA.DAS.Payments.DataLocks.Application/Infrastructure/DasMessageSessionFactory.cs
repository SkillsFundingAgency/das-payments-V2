using System;
using System.Linq;
using Autofac;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Messages.Internal;

namespace SFA.DAS.Payments.DataLocks.Application.Infrastructure
{
    public interface IDasMessageSessionFactory
    {
        IMessageSession Create();
        EndpointConfiguration InitialiseEndpointConfig();
    }

    public class DasMessageSessionFactory : IDasMessageSessionFactory
    {
        private readonly IApplicationConfiguration config;
        private readonly IConfigurationHelper configurationHelper;
        private readonly IPaymentLogger logger;
        private readonly Lazy<IMessageSession> messageSession;

        public DasMessageSessionFactory(IApplicationConfiguration config, IConfigurationHelper configurationHelper, IPaymentLogger logger)
        {
            this.config = config;
            this.configurationHelper = configurationHelper;
            this.logger = logger;
            var endpointConfiguration = InitialiseEndpointConfig();

            messageSession = new Lazy<IMessageSession>(() => Endpoint.Start(endpointConfiguration).Result);
        }

        public IMessageSession Create()
        {
            return messageSession.Value;
        }

        public EndpointConfiguration InitialiseEndpointConfig()
        {
            var endpointConfiguration = new EndpointConfiguration(config.EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions
                .DefiningMessagesAs(t =>
                    t.IsAssignableTo<ApprenticeshipCreatedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipUpdatedApprovedEvent>() ||
                    t.IsAssignableTo<DataLockTriageApprovedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipStoppedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipStopDateChangedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipPausedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipResumedEvent>() ||
                    t.IsAssignableTo<PaymentOrderChangedEvent>()
                )
                .DefiningEventsAs(t =>
                    t.IsAssignableTo<ApprenticeshipCreatedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipUpdatedApprovedEvent>() ||
                    t.IsAssignableTo<DataLockTriageApprovedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipStoppedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipStopDateChangedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipPausedEvent>() ||
                    t.IsAssignableTo<ApprenticeshipResumedEvent>() ||
                    t.IsAssignableTo<PaymentOrderChangedEvent>()
                )
                .DefiningCommandsAs(t => t.IsAssignableTo<PublishDeferredApprovalEventsCommand>())
                ;
                
            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(config.StorageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport
                .ConnectionString(configurationHelper.GetConnectionString("DASServiceBusConnectionString"))
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .RuleNameShortener(ruleName => ruleName.Split('.').LastOrDefault() ?? ruleName);

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(PublishDeferredApprovalEventsCommand), configurationHelper.GetSetting("ApprovalsEndpointName"));

            endpointConfiguration.SendFailedMessagesTo(config.FailedMessagesQueue);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();

            if (config.ProcessMessageSequentially) endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            endpointConfiguration.Pipeline.Register(typeof(ExceptionHandlingBehavior),
                "Logs exceptions to the payments logger");
            
            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(logger));

            return endpointConfiguration;
        }
    }
}
