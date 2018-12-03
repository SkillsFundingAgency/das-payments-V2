using Autofac;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class AuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingSourcePaymentsEventProcessor>()
                .As<IFundingSourcePaymentsEventProcessor>()
                .InstancePerLifetimeScope();
        }
    }
}