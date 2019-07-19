using System;
using NServiceBus;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging
{
    public interface IMonitoringMessageSessionFactory
    {
        IMessageSession Create();
    }

    public class MonitoringMessageSessionFactory: IMonitoringMessageSessionFactory
    {
        private readonly  Lazy<IMessageSession> messageSession;
        
        public MonitoringMessageSessionFactory(EndpointConfiguration endpointConfiguration)
        {
            messageSession = new Lazy<IMessageSession>(() => Endpoint.Start(endpointConfiguration).Result);
        }

        public IMessageSession Create()
        {
            return messageSession.Value;
        }
    }
}