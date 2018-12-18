using Autofac;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application.Handlers;
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

            // app layer event handlers
            builder.RegisterType<ApprenticeshipContractType2PaymentDueEventHandler>().Keyed<IPaymentDueEventHandler>(typeof(ApprenticeshipContractType2PaymentDueEvent));
            builder.RegisterType<IncentivePaymentDueEventHandler>().Keyed<IPaymentDueEventHandler>(typeof(IncentivePaymentDueEvent));
            builder.RegisterType<FunctionalSkillPaymentDueEventHandler>().Keyed<IPaymentDueEventHandler>(typeof(FunctionalSkillPaymentDueEvent));
        }
    }
}