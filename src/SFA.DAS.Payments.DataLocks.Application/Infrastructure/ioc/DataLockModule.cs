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
            builder.RegisterType<ActorReliableCollectionCache<List<ApprenticeshipModel>>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockLearnerCache>().As<IDataLockLearnerCache>().InstancePerLifetimeScope();
            builder.RegisterType<UkprnMatcher>().As<IUkprnMatcher>().InstancePerLifetimeScope();
            builder.RegisterType<UlnLearnerMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UlnLearnerMatcher>().As<IUlnLearnerMatcher>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CourseValidator>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
