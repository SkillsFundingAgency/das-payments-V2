using Autofac;
using SFA.DAS.Payments.Application.Batch;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Application.Handlers;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Application.Services;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Global;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class EarningEventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LearnerSubmissionProcessor>()
                .As<ILearnerSubmissionProcessor>()
                .InstancePerLifetimeScope();
            builder.RegisterType<LearnerValidator>()
                .As<ILearnerValidator>()
                .InstancePerLifetimeScope();
            builder.RegisterType<FM36GlobalValidationRule>()
                .As<IFM36GlobalValidationRule>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipContractTypeEarningsEventBuilder>()
                .As<IApprenticeshipContractTypeEarningsEventBuilder>()
                .InstancePerLifetimeScope();
            builder.RegisterType<FunctionalSkillEarningEventBuilder>()
                .As<IFunctionalSkillEarningsEventBuilder>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipContractTypeEarningsEventFactory>()
                .As<IApprenticeshipContractTypeEarningsEventFactory>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ApprenticeshipContractType2EarningEventsService>()
                .As<IEarningEventsProcessingService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<RedundancyEarningEventFactory>()
                .As<IRedundancyEarningEventFactory>();
            builder.RegisterType<RedundancyEarningService>()
                .As<IRedundancyEarningService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SubmittedLearnerAimBuilder>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<SubmittedLearnerAimBulkCopyConfiguration>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<BulkWriter<SubmittedLearnerAimModel>>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<JobContextMessageHandler>()
                .As<IMessageHandler<JobContextMessage>>();


            builder.RegisterType<SubmittedLearnerAimRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}