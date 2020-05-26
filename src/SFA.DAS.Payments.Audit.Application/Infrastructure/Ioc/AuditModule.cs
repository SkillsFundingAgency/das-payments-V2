using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
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

            builder.RegisterType<AuditDataContextFactory>()
                .As<IAuditDataContextFactory>()
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