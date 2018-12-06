using Autofac;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.PaymentsDue;
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
            builder.RegisterType<PaymentsDueEventProcessor>()
                .As<IPaymentsDueEventProcessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FundingSourceDataTable>()
                .As<IPaymentsEventModelDataTable<FundingSourceEventModel>>();

            builder.RegisterType<PaymentsDueDataTable>()
                .As<IPaymentsEventModelDataTable<PaymentsDueEventModel>>();

            builder.RegisterType<RequiredPaymentDataTable>()
                .As<IPaymentsEventModelDataTable<RequiredPaymentEventModel>>();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchService<>))
                .As(typeof(IPaymentsEventModelBatchService<>))
                .SingleInstance();

            builder.RegisterGeneric(typeof(PaymentsEventModelBatchProcessor<>))
                .As(typeof(IPaymentsEventModelBatchProcessor<>))
                .InstancePerLifetimeScope();
        }
    }
}