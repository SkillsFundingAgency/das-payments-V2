﻿using Autofac;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc.Modules
{
    public class EndpointListenerModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<EndpointCommunicationListener>().As<IEndpointCommunicationListener>();
        }
    }
}