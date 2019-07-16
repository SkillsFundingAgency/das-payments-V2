using System;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.PeriodEnd.Application.Handlers;

namespace SFA.DAS.Payments.PeriodEnd.Application.Infrastructure.Ioc
{
    public class PeriodEndModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeriodEndJobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
        }
    }
}