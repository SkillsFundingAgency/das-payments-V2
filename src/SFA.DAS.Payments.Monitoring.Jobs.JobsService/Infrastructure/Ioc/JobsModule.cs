using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsService.Infrastructure.Ioc
{
    public class JobsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<ActorReliableCollectionCache<JobModel>>()
            //    .As<IActorDataCache<JobModel>>()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ActorReliableCollectionCache<JobStepModel>>()
            //    .As<IActorDataCache<JobStepModel>>()
            //    //.AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ActorReliableCollectionCache<List<Guid>>>()
            //    .As<IActorDataCache<List<Guid>>>()
            //    //.AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();
            //builder.RegisterType<ActorReliableCollectionCache<(JobStepStatus jobStatus, DateTimeOffset? endTime)>>()
            //    .As<IActorDataCache<(JobStepStatus jobStatus, DateTimeOffset? endTime)>>()
            //    //.AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();
        }
    }
}