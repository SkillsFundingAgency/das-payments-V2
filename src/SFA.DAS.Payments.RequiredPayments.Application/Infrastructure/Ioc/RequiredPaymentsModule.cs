﻿using Autofac;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
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

            // app layer event handlers
            builder.RegisterType<ApprenticeshipContractType2EarningEventProcessor>().Keyed<IEarningEventHandler>(typeof(ApprenticeshipContractType2EarningEvent));
            builder.RegisterType<FunctionalSkillEarningsEventProcessor>().Keyed<IEarningEventHandler>(typeof(FunctionalSkillEarningsEvent));
        }
    }
}