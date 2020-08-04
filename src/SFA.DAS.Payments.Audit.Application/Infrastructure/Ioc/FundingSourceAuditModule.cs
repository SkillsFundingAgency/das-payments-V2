using Autofac;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class FundingSourceAuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingSourceEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceEventMapper>()
                .As<IFundingSourceEventMapper>();

            builder.RegisterType<FundingSourceEventStorageService>()
                .As<IFundingSourceEventStorageService>()
                .InstancePerLifetimeScope();
        }
    }
}