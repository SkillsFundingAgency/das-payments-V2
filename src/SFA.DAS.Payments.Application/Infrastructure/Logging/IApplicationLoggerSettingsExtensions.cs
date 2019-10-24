using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using System.Linq;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public static class IApplicationLoggerSettingsExtensions
    {
        public static LogLevel MinimumLogLevel(this IApplicationLoggerSettings settings)
            => settings.ApplicationLoggerOutputSettingsCollection.Min(x => x.MinimumLogLevel);
    }
}