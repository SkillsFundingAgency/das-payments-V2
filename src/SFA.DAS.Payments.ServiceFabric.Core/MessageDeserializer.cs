using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class MessageDeserializer
    {
        public MessageDeserializer(ILifetimeScope lifetimeScope)
        {

        }


        public object DeserializeMessage(Message message)
        {
            if (!message.UserProperties.ContainsKey(NServiceBus.Headers.EnclosedMessageTypes))
                throw new InvalidOperationException($"Cannot deserialise the message, no 'enclosed message types' header was found. Message id: {message.MessageId}, label: {message.Label}");
            var enclosedTypes = (string)message.UserProperties[NServiceBus.Headers.EnclosedMessageTypes];
            var typeName = enclosedTypes.Split(';').FirstOrDefault();
            if (string.IsNullOrEmpty(typeName))
                throw new InvalidOperationException($"Message type not found when trying to deserialise the message.  Message id: {message.MessageId}, label: {message.Label}");
            var messageType = Type.GetType(typeName, assemblyName => { assemblyName.Version = null; return Assembly.Load(assemblyName); }, null);
            var sanitisedMessageJson = GetMessagePayload(message);
            var deserialisedMessage = JsonConvert.DeserializeObject(sanitisedMessageJson, messageType);
            return deserialisedMessage;
        }

        private string GetMessagePayload(Message receivedMessage)
        {
            const string transportEncodingHeaderKey = "NServiceBus.Transport.Encoding";
            var transportEncoding = receivedMessage.UserProperties.ContainsKey(transportEncodingHeaderKey)
                ? (string)receivedMessage.UserProperties[transportEncodingHeaderKey]
                : "application/octet-stream";
            byte[] messageBody;
            if (transportEncoding.Equals("wcf/byte-array", StringComparison.OrdinalIgnoreCase))
            {
                var doc = receivedMessage.GetBody<XmlElement>();
                messageBody = Convert.FromBase64String(doc.InnerText);
            }
            else
                messageBody = receivedMessage.Body;

            var monitoringMessageJson = Encoding.UTF8.GetString(messageBody);
            var sanitisedMessageJson = monitoringMessageJson
                .Trim(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())
                    .ToCharArray());
            return sanitisedMessageJson;
        }
    }

    

    //public class Default
}