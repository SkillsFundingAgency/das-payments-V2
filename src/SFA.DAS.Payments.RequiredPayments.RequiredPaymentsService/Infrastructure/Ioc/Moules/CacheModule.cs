using System.Collections.Generic;
using Autofac;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Infrastructure.Ioc.Moules
{
    public class CacheModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReliableCollectionCache<IEnumerable<PaymentEntity>>>().AsImplementedInterfaces();
            //builder.RegisterType<PaymentHistoryRepository>().AsImplementedInterfaces();
        }
    }
}