using System;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.PeriodEnd.Application.Handlers;
using SFA.DAS.Payments.PeriodEnd.Application.Services;

namespace SFA.DAS.Payments.PeriodEnd.Application.Infrastructure.Ioc
{
    public class PeriodEndModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var config = c.Resolve<IConfigurationHelper>();
                var periodEndConfig = new PeriodEndConfiguration
                {
                    TimeToPauseBetweenChecks = TimeSpan.Parse(config.GetSettingOrDefault("TimeToPauseBetweenChecks", "00:00:30")),
                    TimeToWaitForJobToComplete = TimeSpan.Parse(config.GetSettingOrDefault("TimeToWaitForJobToComplete", "00:02:30"))
                };
                return periodEndConfig;
            })
                .As<IPeriodEndConfiguration>()
                .SingleInstance();
            builder.RegisterType<PeriodEndJobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
            builder.RegisterType<JobStatusService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}