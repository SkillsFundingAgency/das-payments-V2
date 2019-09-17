using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.UnitOfWork;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc.Modules
{
    public class UnitOfWorkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkScope>().As<IUnitOfWorkScope>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWorkScopeFactory>().As<IUnitOfWorkScopeFactory>().InstancePerLifetimeScope();
            builder.RegisterType<StateManagerUnitOfWork>().As<IStateManagerUnitOfWork, StateManagerUnitOfWork>()
                .InstancePerLifetimeScope();
        }
    }
}