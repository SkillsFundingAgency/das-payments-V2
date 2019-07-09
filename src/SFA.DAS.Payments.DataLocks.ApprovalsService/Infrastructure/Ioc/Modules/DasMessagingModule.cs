using Autofac;
using Autofac.Core;
using NServiceBus;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Infrastructure.Ioc.Modules
{
    public class DasMessagingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DasStatelessEndpointCommunicationListener>()
                .WithParameter(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(EndpointConfiguration),
                    (pi, ctx) => ctx.ResolveNamed<EndpointConfiguration>("DASEndpointConfiguration")))
                .As<IDasStatelessEndpointCommunicationListener>()
                .SingleInstance();
        }
    }
}