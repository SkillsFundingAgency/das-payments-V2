﻿using Autofac;
using NServiceBus.UnitOfWork;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.PaymentsEventModelCache;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure.Ioc
{
    public class ServiceFabricModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(PaymentsEventModelCache<>))
                .As(typeof(IPaymentsEventModelCache<>))
                .InstancePerLifetimeScope();
            builder.RegisterType<BatchScope>()
                .As<IBatchScope>();
            builder.RegisterType<BatchScopeFactory>()
                .As<IBatchScopeFactory>();
        }
    }
}