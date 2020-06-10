using Autofac;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Domain.Services;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.EarningEvents.ProcessLearnerService.Infrastructure.Ioc.Modules
{
    public class ProcessLearnerServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.RegisterType<ActorReliableCollectionCache<LearnerKey>>()
                .As<IActorDataCache<LearnerKey>>();

            builder.RegisterType<DuplicateLearnerService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}