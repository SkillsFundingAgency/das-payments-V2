using Autofac;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class EarningEventAuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EarningEventProcessor>()
                .As<IEarningEventProcessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EarningEventSubmissionSucceededProcessor>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<EarningEventSubmissionFailedProcessor>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<EarningEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            builder.RegisterType<EarningEventStorageService>()
                .As<IEarningEventStorageService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EarningEventMapper>()
                .As<IEarningEventMapper>();

            builder.RegisterType<EarningsDuplicateEliminator>()
                .As<IEarningsDuplicateEliminator>()
                .InstancePerLifetimeScope();

        }
    }
}