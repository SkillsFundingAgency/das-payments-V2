using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Configuration
{
    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public string EndpointName { get; set; }
        public string StorageConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string FailedMessagesQueue { get; set; }
    }
}