using System;
using NServiceBus;

namespace SFA.DAS.Payments.FundingSource.LevyTransactionService.Infrastructure.Messaging
{
    public interface IMessageSessionFactory
    {
        IMessageSession Create();
    }

    public class MessageSessionFactory: IMessageSessionFactory
    {
        private readonly Lazy<IMessageSession> messageSession;

        public MessageSessionFactory(EndpointConfiguration endpointConfiguration)
        {
            messageSession = new Lazy<IMessageSession>(() => Endpoint.Start(endpointConfiguration).Result);
        }

        public IMessageSession Create()
        {
            return messageSession.Value;
        }
    }
}