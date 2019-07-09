using Autofac;
using SFA.DAS.Payments.DataLocks.Application.Cache;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System.Collections.Generic;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.DataLocks.Application.Infrastructure.ioc
{
    public class DataLockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorReliableCollectionCache<List<ApprenticeshipModel>>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockLearnerCache>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UkprnMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UlnLearnerMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LearnerMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(ICourseValidator).Assembly).As<ICourseValidator>().InstancePerLifetimeScope();

            builder.RegisterType<EarningPeriodsValidationProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CourseValidationProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CalculatePeriodStartAndEndDate>().AsImplementedInterfaces().InstancePerLifetimeScope();


            builder.RegisterType<BatchProcessor<DataLockStatusChanged>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<StateManagerUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchedDataCache<DataLockStatusChanged>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CachingEventProcessor<DataLockStatusChanged>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableStateManagerTransactionProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableStateManagerProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchScope>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchScopeFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchProcessingService<DataLockStatusChanged>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BulkWriter<DataLockStatusChanged>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockStatusChangedBulkCopyConfiguration>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
