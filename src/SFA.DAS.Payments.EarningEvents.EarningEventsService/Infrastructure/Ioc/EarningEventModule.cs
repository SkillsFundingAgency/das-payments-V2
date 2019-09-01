using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.EarningEvents.EarningEventsService.Handlers;

namespace SFA.DAS.Payments.EarningEvents.EarningEventsService.Infrastructure.Ioc
{
    public class EarningEventModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobContextMessageHandler>()
                .As<IMessageHandler<JobContextMessage>>();
        }
    }
}