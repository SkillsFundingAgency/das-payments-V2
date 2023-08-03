using SFA.DAS.Payments.Core.Configuration;
using System;

namespace SFA.DAS.Payments.Application.Infrastructure.Configuration
{
    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public string EndpointName { get; set; }
        public string StorageConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string FailedMessagesQueue { get; set; }
        public bool ProcessMessageSequentially { get; set; }
        public string NServiceBusLicense { get; set; }
        public int ImmediateMessageRetries { get; set; }
        public int DelayedMessageRetries { get; set; }
        public TimeSpan DelayedMessageRetryDelay { get; set; }

        //TODO: Needed somewhere to put this, will refactor if this works for the new NSB config API.
        public static IApplicationConfiguration Create(IConfigurationHelper configHelper)
        {
            bool.TryParse(configHelper.GetSettingOrDefault("ProcessMessageSequentially", "false"), out bool processMessageSequentially);

            if (!TimeSpan.TryParse(configHelper.GetSettingOrDefault("DelayedMessageRetryDelay", "00:00:10"), out var delayedRetryDelay))
                delayedRetryDelay = new TimeSpan(0, 0, 10);

            return new ApplicationConfiguration
            {
                EndpointName = configHelper.GetSetting("EndpointName"),
                StorageConnectionString = configHelper.GetConnectionString("StorageConnectionString"),
                ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString"),
                FailedMessagesQueue = configHelper.GetSetting("FailedMessagesQueue"),
                ProcessMessageSequentially = processMessageSequentially,
                NServiceBusLicense = configHelper.GetSetting("DasNServiceBusLicenseKey"),
                ImmediateMessageRetries = configHelper.GetSettingOrDefault("ImmediateMessageRetries", 1),
                DelayedMessageRetries = configHelper.GetSettingOrDefault("DelayedMessageRetries", 3),
                DelayedMessageRetryDelay = delayedRetryDelay,
            };
        }
    }
}