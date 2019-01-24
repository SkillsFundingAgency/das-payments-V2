using Autofac;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ContainerScopeFactory>()
                .As<IContainerScopeFactory>()
                .SingleInstance();
        }
    }
}