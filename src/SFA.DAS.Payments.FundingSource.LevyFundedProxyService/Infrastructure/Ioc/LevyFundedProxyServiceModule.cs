using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Infrastructure.Ioc
{
    public class LevyFundedProxyServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterBuildCallback(c => c.Resolve<EndpointConfiguration>()
                .Pipeline.Register(typeof(MessageTimeOutBehaviour), "Behaviour to handle message lock renewal and timeout"));

        }
    }
}