using System.Collections.Generic;
using Autofac;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Cache;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.DataLocks.Application.Infrastructure.ioc
{
    public class DataLockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorReliableCollectionCache<List<ApprenticeshipModel>>>().As<IActorDataCache<ApprenticeshipModel>>().InstancePerLifetimeScope();
            builder.RegisterType<DataLockLearnerCache>().As<IDataLockLearnerCache>().InstancePerLifetimeScope();
            builder.RegisterType<UkprnMatcher>().As<IUkprnMatcher>().InstancePerLifetimeScope();
        }
    }
}
