namespace SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration
{
    public interface IServiceConfig
    {
        string StorageConnectionString { get; }
        string IncomingEndpointName { get; }
        string OutgoingEndpointName { get; }
        string DestinationEndpointName { get; }
    }
}