using Autofac;
using NServiceBus.UnitOfWork;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure.Ioc
{
    public class ServiceFabricModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StateManagerUnitOfWork>().As<IManageUnitsOfWork>();
        }
    }
}