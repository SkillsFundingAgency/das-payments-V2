using Autofac;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Audit.EarningEventsService.Handlers;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.EarningEventsService.Infrastructure.Ioc
{
    public class AuditEarningEventServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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
        }
    }
}