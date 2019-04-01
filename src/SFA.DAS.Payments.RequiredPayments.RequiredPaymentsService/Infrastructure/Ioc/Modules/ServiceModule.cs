using Autofac;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Infrastructure.Ioc.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReliableCollectionCache<PaymentHistoryEntity[]>>().AsImplementedInterfaces();
            builder.RegisterType<ActorApprenticeshipKeyProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}