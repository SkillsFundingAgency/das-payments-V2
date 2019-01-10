using Autofac;
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
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new LoggerOptions
                    {
                        LoggerConnectionString = configHelper.GetConnectionString("LoggingConnectionString")
                    };
                })
                .As<LoggerOptions>()
                .SingleInstance();
            builder.RegisterType<VersionInfo>().As<IVersionInfo>().SingleInstance();
            builder.Register(c =>
                {
                    var loggerOptions = c.Resolve<LoggerOptions>();
                    var versionInfo = c.Resolve<IVersionInfo>();
                    var configHelper = c.Resolve<IConfigurationHelper>();

                    if (!Enum.TryParse(configHelper.GetSetting("LogLevel"),  out LogLevel LogLevel))
                    {
                        LogLevel = LogLevel.Information;
                    }
                    
                    return new ApplicationLoggerSettings
                    {
                        ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                        {
                            new MsSqlServerApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = LogLevel,
                                ConnectionString = loggerOptions.LoggerConnectionString,
                            },

                            new ConsoleApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = LogLevel
                            },
                        },
                        TaskKey = versionInfo.ServiceReleaseVersion
                    };
                })
                .As<IApplicationLoggerSettings>()
                .SingleInstance();
            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<ExecutionContextFactory>().As<IExecutionContextFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentLogger>()
                .As<IPaymentLogger, ILogger>()
                .InstancePerLifetimeScope();
        }
    }
}