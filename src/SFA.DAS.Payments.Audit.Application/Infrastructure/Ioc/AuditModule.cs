using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class AuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingSourcePaymentsEventProcessor>()
                .As<IFundingSourcePaymentsEventProcessor>()
                .InstancePerLifetimeScope();
            builder.RegisterType<FundingSourceDataTable>()
                .As<IPaymentsEventModelDataTable<FundingSourceEventModel>>();
            builder.RegisterGeneric(typeof(PaymentsEventModelBatchService<>))
                .As(typeof(IPaymentsEventModelBatchService<>))
                .SingleInstance();
            builder.RegisterGeneric(typeof(PaymentsEventModelBatchProcessor<>))
                .As(typeof(IPaymentsEventModelBatchProcessor<>))
                .InstancePerLifetimeScope();
        }
    }
}