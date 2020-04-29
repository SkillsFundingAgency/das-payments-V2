using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.EarningEventsService.Handlers;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messaging.Serialization;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Infrastructure.Ioc
{
    public class AuditEarningEventServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var appConfig = c.Resolve<IApplicationConfiguration>();
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new StatelessServiceBusBatchCommunicationListener(configHelper.GetConnectionString("ServiceBusConnectionString"),
                        appConfig.EndpointName,
                        appConfig.FailedMessagesQueue,
                        c.Resolve<IPaymentLogger>(),
                        c.Resolve<IContainerScopeFactory>(),
                        c.Resolve<ITelemetry>(),
                        c.Resolve<IMessageDeserializer>(), 
                        c.Resolve<IApplicationMessageModifier>());
                })
                .As<IStatelessServiceBusBatchCommunicationListener>()
                .SingleInstance();

            builder.RegisterType<ApprenticeshipContractType1EarningEventHandler>()
                .As<IHandleMessageBatches<ApprenticeshipContractType1EarningEvent>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ApprenticeshipContractType2EarningEventHandler>()
                .As<IHandleMessageBatches<ApprenticeshipContractType2EarningEvent>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<Act1FunctionalSkillEarningsEventHandler>()
                .As<IHandleMessageBatches<Act1FunctionalSkillEarningsEvent>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<Act2FunctionalSkillEarningsEventHandler>()
                .As<IHandleMessageBatches<Act2FunctionalSkillEarningsEvent>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EarningEventModelHandler>()
                .As<IHandleMessageBatches<EarningEventModel>>()
                .InstancePerLifetimeScope();
        }
    }
}