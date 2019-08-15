using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class AuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FundingSourcePaymentsEventProcessor>()
                .As<IFundingSourcePaymentsEventProcessor>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RequiredPaymentEventProcessor>()
                .As<IRequiredPaymentEventProcessor>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EarningEventProcessor>()
                .As<IEarningEventProcessor>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DataLockEventProcessor>()
                .As<IDataLockEventProcessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceDataTable>()
                .As<IPaymentsEventModelDataTable<FundingSourceEventModel>>();

            builder.RegisterType<RequiredPaymentDataTable>()
                .As<IPaymentsEventModelDataTable<RequiredPaymentEventModel>>();

            builder.RegisterType<EarningEventDataTable>()
                .As<IPaymentsEventModelDataTable<EarningEventModel>>();

            builder.RegisterType<DataLockEventDataTable>()
                .As<IPaymentsEventModelDataTable<DataLockEventModel>>();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchService<>))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchProcessor<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}