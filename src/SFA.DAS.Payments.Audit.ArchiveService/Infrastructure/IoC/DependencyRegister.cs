﻿using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using ConfigurationModule = SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC.Modules.ConfigurationModule;

namespace SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC
{
    public class DependencyRegister
    {
        public DependencyRegister(string functionName)
        {
            DependencyInjection.Initialize(RegisterModules, functionName);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<TelemetryModule>();
            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<ConfigurationModule>();
        }
    }
}