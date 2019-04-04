using System.Collections.Generic;
using Autofac;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.DataLocks.Application.Infrastructure.ioc
{
    public class DataLockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ReliableCollectionCache<List<ApprenticeshipModel>>>().AsImplementedInterfaces();
        }
    }
}
