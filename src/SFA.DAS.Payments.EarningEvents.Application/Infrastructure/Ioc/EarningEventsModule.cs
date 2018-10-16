using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Application.Services;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Global;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class EarningEventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LearnerSubmissionProcessor>()
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
            builder.RegisterType<ApprenticeshipContractTypeEarningsEventFactory>()
                .As<IApprenticeshipContractTypeEarningsEventFactory>()
                .InstancePerLifetimeScope();

            builder.Register(c => new ApprenticeshipContractType2EarningEventsService
            (
                c.Resolve<IPaymentLogger>(),
                c.Resolve<IEarningEventMapper>()
            )).As<IEarningEventsProcessingService>();
        }
    }
}