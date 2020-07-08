using Autofac;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment;

namespace SFA.DAS.Payments.Audit.Application.Infrastructure.Ioc
{
    public class RequiredPaymentAuditModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequiredPaymentEventRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequiredPaymentEventMapper>()
                .As<IRequiredPaymentEventMapper>();

            builder.RegisterType<RequiredPaymentEventStorageService>()
                .As<IRequiredPaymentEventStorageService>()
                .InstancePerLifetimeScope();
        }
    }
}