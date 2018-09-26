using Autofac;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Infrastructure.Ioc.Modules
{
    public class CacheModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReliableCollectionCache<PaymentEntity[]>>().AsImplementedInterfaces();
        }
    }
}