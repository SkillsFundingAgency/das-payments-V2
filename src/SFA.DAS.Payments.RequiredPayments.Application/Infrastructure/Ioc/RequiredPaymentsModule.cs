using Autofac;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Ioc
{
    public class RequiredPaymentsModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApprenticeshipKeyService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<PaymentKeyService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<PaymentDueProcessor>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RequiredPaymentProcessor>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RefundService>().AsImplementedInterfaces().SingleInstance();

            // app layer event handlers
            builder.RegisterType<ApprenticeshipContractType2EarningEventProcessor>().AsImplementedInterfaces();
            builder.RegisterType<FunctionalSkillEarningsEventProcessor>().AsImplementedInterfaces();
            builder.RegisterType<PayableEarningEventProcessor>().AsImplementedInterfaces();
        }
    }
}