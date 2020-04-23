using Autofac;
using SFA.DAS.Payments.DataLocks.Application.Cache;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.DataLocks.Application.Infrastructure.ioc
{
    public class DataLockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorReliableCollectionCache<List<ApprenticeshipModel>>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ActorReliableCollectionCache<List<long>>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockLearnerCache>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UkprnMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<UlnLearnerMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LearnerMatcher>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(ICourseValidator).Assembly).As<ICourseValidator>().InstancePerLifetimeScope();

            builder.RegisterType<OnProgrammeAndIncentiveEarningPeriodsValidationProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CourseValidationProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<CompletionStoppedValidator>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<OnProgrammeAndIncentiveStoppedValidator>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register(ctx =>
            {
                var configHelper = ctx.Resolve<IConfigurationHelper>();
                var disableDatalocks = configHelper.GetSettingOrDefault("DisableDatalocks", false);
                
                // If datalocks are disabled (PV2-1887) then the only validator that we want
                //  enabled is the Pause validator (DLOCK_12)
                if (disableDatalocks)
                {
                    return new CourseValidationProcessor(
                        new List<ICourseValidator> {new ApprenticeshipPauseValidator()});
                }

                // Otherwise all of the validators
                return new CourseValidationProcessor(ctx.Resolve<IEnumerable<ICourseValidator>>().ToList());
            }).AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register(ctx =>
            {
                var configHelper = ctx.Resolve<IConfigurationHelper>();
                var disableDatalocks = configHelper.GetSettingOrDefault("DisableDatalocks", false);

                return new StartDateValidator(disableDatalocks);
            }).AsImplementedInterfaces().InstancePerLifetimeScope();
            
            builder.RegisterType<CalculatePeriodStartAndEndDate>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockStatusService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<FunctionalSkillEarningPeriodsValidationProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<BatchedDataCache<PriceEpisodeStatusChange>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<CachingEventProcessor<PriceEpisodeStatusChange>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ReliableStateManagerTransactionProvider>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchScope>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchScopeFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<BatchProcessingService<PriceEpisodeStatusChange>>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DataLockStatusChangedEventBatchProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<LegacyDataLockEventBulkCopyConfiguration>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LegacyDataLockEventCommitmentVersionBulkCopyConfiguration>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LegacyDataLockEventErrorBulkCopyConfiguration>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<LegacyDataLockEventPeriodBulkCopyConfiguration>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register(ctx => new FunctionalSkillValidationProcessor(new List<ICourseValidator>
            {
                new ApprenticeshipPauseValidator()
            })).As<IFunctionalSkillValidationProcessor>().InstancePerLifetimeScope();

            builder.RegisterType<DataLockProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<ApprenticeshipProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipUpdatedProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipApprovedUpdatedService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipDataLockTriageService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipStoppedService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipPauseService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipResumedService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<PriceEpisodeStatusChangeBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<PriceEpisodesReceivedService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ManageReceivedDataLockEvent>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}