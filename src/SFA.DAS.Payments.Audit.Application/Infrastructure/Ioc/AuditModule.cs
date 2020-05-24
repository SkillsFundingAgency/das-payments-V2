using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.Data.DataLock;
using SFA.DAS.Payments.Audit.Application.Data.EarningEvent;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents;
using SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class AuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EarningEventProcessor>()
                .As<IEarningEventProcessor>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchService<>))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchProcessor<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<EarningEventSubmissionSucceededProcessor>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<EarningEventSubmissionFailedProcessor>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<DataLockEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequiredPaymentEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceEventRepository>()
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

            builder.RegisterType<RequiredPaymentEventMapper>()
                .As<IRequiredPaymentEventMapper>();

            builder.RegisterType<EarningsDuplicateEliminator>()
                .As<IEarningsDuplicateEliminator>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AuditDataContextFactory>()
                .As<IAuditDataContextFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DataLockEventStorageService>()
                .As<IDataLockEventStorageService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequiredPaymentEventStorageService>()
                .As<IDataLockEventStorageService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceEventStorageService>()
                .As<IDataLockEventStorageService>()
                .InstancePerLifetimeScope();

            builder.Register(ctx =>
                {
                    var configHelper = ctx.Resolve<IConfigurationHelper>();
                    var dbContext = new AuditDataContext(configHelper.GetConnectionString("PaymentsConnectionString"));
                    return dbContext;
                })
                .As<IAuditDataContext>()
                .InstancePerDependency();
        }
    }
}