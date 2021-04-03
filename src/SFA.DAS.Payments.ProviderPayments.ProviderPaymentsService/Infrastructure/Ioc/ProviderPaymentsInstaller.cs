using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Messaging.Serialization;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.MessageInterceptors;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.MessageModifiers;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Infrastructure.Ioc
{
    public class ProviderPaymentsInstaller: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DasEndpointFactory>()
                   .As<IDasEndpointFactory>()
                   .SingleInstance();

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

            builder.RegisterType<MessageModifier>().As<IApplicationMessageModifier>();

            //builder.RegisterType<FundingSourcePaymentEventBatchHandler>()
            //    .As<IHandleMessageBatches<FundingSourcePaymentEvent>>()
            //    .InstancePerLifetimeScope();
            
            builder.RegisterType<PaymentModelBatchHandler>()
                .As<IHandleMessageBatches<PaymentModel>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PeriodEndStoppedEventHandler>()
                .As<IHandleMessageBatches<PeriodEndStoppedEvent>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PublishProviderAct1CompletionPaymentsCommandHandler>()
                .As<IHandleMessageBatches<PublishProviderAct1CompletionPaymentsCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubmissionFailedEventHandler>()
                .As<IHandleMessageBatches<SubmissionJobFailed>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SubmissionSucceededEventHandler>()
                .As<IHandleMessageBatches<SubmissionJobSucceeded>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<InterceptSuccessfulJobMessages>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<InterceptFailedJobMessages>()
                .AsImplementedInterfaces()
                .SingleInstance();


        }
    }
}