using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messaging.Serialization;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers;
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

            //builder.RegisterType<IlrSubmissionCache>()
            //       .As<IDataCache<ReceivedProviderEarningsEvent>>()
            //       .InstancePerLifetimeScope();
            //builder.RegisterType<MonthEndCache>()
            //    .As<IMonthEndCache>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterBuildCallback(c =>
            //{
            //    var recoverability = c.Resolve<EndpointConfiguration>()
            //        .Recoverability();
            //    recoverability.Immediate(immediate => immediate.NumberOfRetries(3));
            //});

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

            builder.RegisterType<PaymentModelBatchHandler>()
                .As<IHandleMessageBatches<PaymentModel>>()
                .InstancePerLifetimeScope();
            //builder.RegisterType<FundingSourceEventMessageModifier>()
            //    .As<IApplicationMessageModifier>();

            //builder.RegisterType<FundingSourceEventModelHandler>()
            //    .As<IHandleMessageBatches<FundingSourceEventModel>>()
            //    .InstancePerLifetimeScope();
        }
    }
}