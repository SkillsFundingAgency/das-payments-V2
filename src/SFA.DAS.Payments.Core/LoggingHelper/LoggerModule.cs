using Autofac;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Core.LoggingHelper
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)

        {
            builder.Register(c =>

            {
                var loggerOptions = c.Resolve<LoggerOptions>();

                var versionInfo = c.Resolve<IVersionInfo>();

                return new ApplicationLoggerSettings

                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()

                    {
                        new MsSqlServerApplicationLoggerOutputSettings()

                        {
                            MinimumLogLevel = LogLevel.Verbose,

                            ConnectionString = loggerOptions.LoggerConnectionstring,
                        },

                        new ConsoleApplicationLoggerOutputSettings()

                        {
                            MinimumLogLevel = LogLevel.Verbose,
                        },
                    },

                    TaskKey = versionInfo.ServiceReleaseVersion
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();
            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentLogger>().As<IPaymentLogger>().InstancePerLifetimeScope();
           
        }
    }

}