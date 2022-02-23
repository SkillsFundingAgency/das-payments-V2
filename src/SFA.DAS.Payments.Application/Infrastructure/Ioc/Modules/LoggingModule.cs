﻿using Autofac;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VersionInfo>().As<IVersionInfo>().SingleInstance();
            builder.RegisterType<PaymentsLoggerConfigurationBuilder>().As<ILoggerConfigurationBuilder>().InstancePerLifetimeScope();
            builder.Register(c =>
                {
                    var versionInfo = c.Resolve<IVersionInfo>();
                    var configHelper = c.Resolve<IConfigurationHelper>();

                    if (!Enum.TryParse(configHelper.GetSettingOrDefault("LogLevel", "Information"),  out LogLevel logLevel))
                    {
                        logLevel = LogLevel.Information;
                    }

                    return new ApplicationLoggerSettings
                    {
                        ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                        {
                            new ConsoleApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = logLevel
                            },
                        },
                        TaskKey = versionInfo.ServiceReleaseVersion
                    };
                })
                .As<IApplicationLoggerSettings>()
                .SingleInstance();
            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<ExecutionContextFactory>().As<IExecutionContextFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentsSerilogLoggerFactory>()
                .UsingConstructor(typeof(ILoggerConfigurationBuilder))
                .As<ISerilogLoggerFactory>()
                .InstancePerLifetimeScope();
            builder.RegisterType<PaymentLogger>()
                .As<IPaymentLogger, ILogger>()
                .InstancePerLifetimeScope();
        }
    }
}