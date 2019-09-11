using System;
using NServiceBus.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    internal class MessagingLogger : ILog
    {
        private readonly IPaymentLogger logger;

        public MessagingLogger(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // IPaymentLogger does not expose the minimum level it is configured for,
        // so we must enable all levels and bear the cost of low-level messages being
        // discarded by IPaymenLogger.
        public bool IsDebugEnabled => true;

        public bool IsInfoEnabled => true;
        public bool IsWarnEnabled => true;
        public bool IsErrorEnabled => true;
        public bool IsFatalEnabled => true;

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

        public void Warn(string message, Exception exception) => logger.LogWarning(message);

        public void WarnFormat(string format, params object[] args) => logger.LogWarning(format, args);
    }
}