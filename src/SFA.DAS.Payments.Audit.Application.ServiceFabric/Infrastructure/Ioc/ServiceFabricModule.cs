using Autofac;
using NServiceBus.UnitOfWork;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventProcessing;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure.Ioc
{
    public class ServiceFabricModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StateManagerUnitOfWork>()
                .As<IManageUnitsOfWork>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ReliableStateManagerTransactionProvider>()
                .As<IReliableStateManagerTransactionProvider>()
                .InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(PaymentsEventModelCache<>))
                .As(typeof(IPaymentsEventModelCache<>))
                .InstancePerLifetimeScope();
            builder.RegisterType<BatchScope>()
                .As<IBatchScope>();
        }
    }
}