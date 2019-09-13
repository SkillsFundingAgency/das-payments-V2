using ESFA.DC.Logging.Config.Interfaces;
using NServiceBus.Logging;
using System;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    internal class MessagingLogger : ILog
    {
        private readonly IPaymentLogger logger;
        private readonly ESFA.DC.Logging.Enums.LogLevel minimum;

        public MessagingLogger(IPaymentLogger logger, IApplicationLoggerSettings settings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            minimum = settings.MinimumLogLevel();
        }

        public bool IsDebugEnabled => minimum >= ESFA.DC.Logging.Enums.LogLevel.Debug;

        public bool IsInfoEnabled => minimum >= ESFA.DC.Logging.Enums.LogLevel.Information;

        public bool IsWarnEnabled => minimum >= ESFA.DC.Logging.Enums.LogLevel.Warning;

        public bool IsErrorEnabled => minimum >= ESFA.DC.Logging.Enums.LogLevel.Error;

        public bool IsFatalEnabled => minimum >= ESFA.DC.Logging.Enums.LogLevel.Fatal;

        public void Debug(string message) => DebugFormat(message);

        public void Debug(string message, Exception exception) => Debug(message);

        public void DebugFormat(string format, params object[] args) => logger.LogDebug(format, args);

        public void Error(string message) => logger.LogError(message);

        public void Error(string message, Exception exception) => logger.LogError(message, exception);

        public void ErrorFormat(string format, params object[] args) => logger.LogError(format, parameters: args);

        public void Fatal(string message) => logger.LogFatal(message);

        public void Fatal(string message, Exception exception) => logger.LogFatal(message, exception);

        public void FatalFormat(string format, params object[] args) => logger.LogFatal(format, parameters: args);

        public void Info(string message) => logger.LogInfo(message);

        public void Info(string message, Exception exception) => logger.LogInfo(message);

        public void InfoFormat(string format, params object[] args) => logger.LogInfo(format, args);

        public void Warn(string message) => logger.LogWarning(message);

        public void Warn(string message, Exception exception) => WarnFormat("{0}\n{1}", message, exception.Message);

        public void WarnFormat(string format, params object[] args) => logger.LogWarning(format, args);
    }
}