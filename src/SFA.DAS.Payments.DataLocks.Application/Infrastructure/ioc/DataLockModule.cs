using System;
using Autofac;
using SFA.DAS.Payments.DataLocks.Application.Cache;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Domain.Services.CourseValidation;
using SFA.DAS.Payments.DataLocks.Domain.Services.LearnerMatching;

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

            builder.RegisterType<CalculatePeriodStartAndEndDate>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.Register(ctx => new FunctionalSkillValidationProcessor(new List<ICourseValidator>
            {
                new CompletionStoppedValidator(),
                new OnProgrammeAndIncentiveStoppedValidator(ctx.Resolve<ICalculatePeriodStartAndEndDate>()),
                new ApprenticeshipPauseValidator()
            })).AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<DataLockProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
          
            builder.RegisterType<ApprenticeshipProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipUpdatedProcessor>().AsImplementedInterfaces().InstancePerLifetimeScope();


            builder.RegisterType<ApprenticeshipApprovedUpdatedService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipDataLockTriageService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipStoppedService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
