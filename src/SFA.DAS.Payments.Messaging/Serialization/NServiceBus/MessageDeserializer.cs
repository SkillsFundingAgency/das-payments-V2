using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Autofac;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.Messaging.Serialization.NServiceBus
{
    public class MessageDeserializer: IMessageDeserializer
    {
        public MessageDeserializer(ILifetimeScope lifetimeScope)
        {

        }


        public object DeserializeMessage(ServiceBusReceivedMessage message)
        {
            if (!message.ApplicationProperties.ContainsKey(global::NServiceBus.Headers.EnclosedMessageTypes))
                throw new InvalidOperationException($"Cannot deserialise the message, no 'enclosed message types' header was found. Message id: {message.MessageId}");
            var enclosedTypes = (string)message.ApplicationProperties[global::NServiceBus.Headers.EnclosedMessageTypes];
            var typeName = enclosedTypes.Split(';').FirstOrDefault();
            if (string.IsNullOrEmpty(typeName))
                throw new InvalidOperationException($"Message type not found when trying to deserialise the message.  Message id: {message.MessageId}");
            var messageType = Type.GetType(typeName, assemblyName => { assemblyName.Version = null; return Assembly.Load(assemblyName); }, null);
            var sanitisedMessageJson = GetMessagePayload(message);
            var deserialisedMessage = JsonConvert.DeserializeObject(sanitisedMessageJson, messageType);
            return deserialisedMessage;
        }

        private string GetMessagePayload(ServiceBusReceivedMessage receivedMessage)
        {
            const string transportEncodingHeaderKey = "NServiceBus.Transport.Encoding";
            var transportEncoding = receivedMessage.ApplicationProperties.ContainsKey(transportEncodingHeaderKey)
                ? (string)receivedMessage.ApplicationProperties[transportEncodingHeaderKey]
                : "application/octet-stream";
            byte[] messageBody;
            return receivedMessage.ToString().Trim(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())
                .ToCharArray());
            //if (transportEncoding.Equals("wcf/byte-array", StringComparison.OrdinalIgnoreCase))
            //{
            //    var doc = receivedMessage.GetBody<XmlElement>();
            //    messageBody = Convert.FromBase64String(doc.InnerText);
            //}
            //else
            //    messageBody = receivedMessage.Body;

            //var monitoringMessageJson = Encoding.UTF8.GetString(messageBody);
            //var sanitisedMessageJson = monitoringMessageJson
            //    .Trim(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())
            //        .ToCharArray());
            //return sanitisedMessageJson;
        }
    }
}