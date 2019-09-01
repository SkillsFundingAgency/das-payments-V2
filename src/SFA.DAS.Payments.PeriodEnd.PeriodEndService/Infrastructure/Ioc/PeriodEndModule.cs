using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.PeriodEnd.PeriodEndService.Handlers;

namespace SFA.DAS.Payments.PeriodEnd.PeriodEndService.Infrastructure.Ioc
{
    public class PeriodEndModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeriodEndJobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();
        }
    }
}