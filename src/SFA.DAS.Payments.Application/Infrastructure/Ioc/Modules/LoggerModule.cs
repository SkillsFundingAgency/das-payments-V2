using System.Collections.Generic;
using Autofac;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)

        {
            builder.Register((c, p) => new LoggerOptions
                {
                    //TODO: Get connection string from config
                    LoggerConnectionString = "Server=.;Database=AppLog;User Id=SFActor; Password=SFActor;"
                })
                .As<LoggerOptions>()
                .SingleInstance();
            builder.RegisterType<VersionInfo>().As<IVersionInfo>().SingleInstance();
            builder.Register(c =>
                {
                    var loggerOptions = c.Resolve<LoggerOptions>();
                    var versionInfo = c.Resolve<IVersionInfo>();
                    return new ApplicationLoggerSettings
                    {
                        ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                        {
                            new MsSqlServerApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = LogLevel.Verbose,
                                ConnectionString = loggerOptions.LoggerConnectionString,
                            },

                            new ConsoleApplicationLoggerOutputSettings
                            {
                                MinimumLogLevel = LogLevel.Verbose,
                            },
                        },
                        TaskKey = versionInfo.ServiceReleaseVersion
                    };
                })
                .As<IApplicationLoggerSettings>()
                .SingleInstance();
            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentLogger>().As<IPaymentLogger>().InstancePerLifetimeScope();
        }
    }

}