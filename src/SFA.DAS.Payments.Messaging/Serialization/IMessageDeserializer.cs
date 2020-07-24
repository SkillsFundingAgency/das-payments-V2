using Microsoft.Azure.ServiceBus;

namespace SFA.DAS.Payments.Messaging.Serialization
{
    public interface IMessageDeserializer
    {
        object DeserializeMessage(Message message);
    }
}