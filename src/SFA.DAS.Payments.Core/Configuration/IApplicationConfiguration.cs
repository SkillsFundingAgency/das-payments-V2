namespace SFA.DAS.Payments.Core.Configuration
{
    public interface IApplicationConfiguration
    {
        string EndpointName { get; }
        string StorageConnectionString { get; }
        string ServiceBusConnectionString { get; }
    }
}