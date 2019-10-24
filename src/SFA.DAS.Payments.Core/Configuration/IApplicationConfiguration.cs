using System;

namespace SFA.DAS.Payments.Core.Configuration
{
    public interface IApplicationConfiguration
    {
        string EndpointName { get; }
        string StorageConnectionString { get; }
        string ServiceBusConnectionString { get; }
        string FailedMessagesQueue { get; }
        bool ProcessMessageSequentially { get; }
        string NServiceBusLicense { get; }
        int ImmediateMessageRetries { get; }
        int DelayedMessageRetries { get; }
        TimeSpan DelayedMessageRetryDelay { get; }
    }
}