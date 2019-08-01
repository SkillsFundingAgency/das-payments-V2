using System;

namespace SFA.DAS.Payments.Core.Logging
{
    public interface ILogger
    {
        void LogFatal(string message, Exception exception);

        void LogError(string message, Exception exception);

        void LogWarning(string message);

        void LogDebug(string message);

        void LogInfo(string message);

        void LogVerbose(string message);
    }
}