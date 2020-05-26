using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
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
            builder.RegisterGeneric(typeof(PaymentsEventModelBatchService<>))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchProcessor<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();


            builder.RegisterType<RequiredPaymentEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequiredPaymentEventMapper>()
                .As<IRequiredPaymentEventMapper>();

            builder.RegisterType<FundingSourceEventMapper>()
                .As<IFundingSourceEventMapper>();

            builder.RegisterType<AuditDataContextFactory>()
                .As<IAuditDataContextFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequiredPaymentEventStorageService>()
                .As<IRequiredPaymentEventStorageService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceEventStorageService>()
                .As<IFundingSourceEventStorageService>()
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