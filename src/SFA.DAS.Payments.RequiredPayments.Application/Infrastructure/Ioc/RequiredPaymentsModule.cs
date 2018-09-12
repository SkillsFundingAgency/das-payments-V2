using Autofac;
using SFA.DAS.Payments.RequiredPayments.Application.Handlers;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Ioc
{
    public class RequiredPaymentsModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApprenticeshipKeyService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<PaymentDueEventHanlder>().AsImplementedInterfaces().SingleInstance();
        }
    }
}