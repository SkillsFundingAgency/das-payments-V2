using Autofac;
using SFA.DAS.Payments.PaymentsDue.Application.Services;
using SFA.DAS.Payments.PaymentsDue.Domain;

namespace SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Ioc
{
    public class PaymentsDueModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IncentiveProcessor>().AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<ApprenticeshipContractType2PayableEarningService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ApprenticeshipContractType2EarningProcessor>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FunctionalSkillsEarningService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FunctionalSkillsEarningProcessor>().AsImplementedInterfaces().SingleInstance();
        }
    }
}