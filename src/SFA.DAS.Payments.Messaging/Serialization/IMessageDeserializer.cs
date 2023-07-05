using Azure.Messaging.ServiceBus;

namespace SFA.DAS.Payments.Messaging.Serialization
{
    public interface IMessageDeserializer
    {
        object DeserializeMessage(Azure.Messaging.ServiceBus.ServiceBusReceivedMessage message);
    }
}