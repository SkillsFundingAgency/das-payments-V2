using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Autofac;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class MessageDeserializer
    {
        public MessageDeserializer(ILifetimeScope lifetimeScope)
        {

        }


        public object DeserializeMessage(ServiceBusReceivedMessage message)
        {
            if (!message.ApplicationProperties.ContainsKey(NServiceBus.Headers.EnclosedMessageTypes))
                throw new InvalidOperationException($"Cannot deserialise the message, no 'enclosed message types' header was found. Message id: {message.MessageId}");
            var enclosedTypes = (string)message.ApplicationProperties[NServiceBus.Headers.EnclosedMessageTypes];
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
            var monitoringMessageJson = receivedMessage.ToString();
            if (transportEncoding.Equals("wcf/byte-array", StringComparison.OrdinalIgnoreCase))
            {
                var document = new XmlDocument();
                document.LoadXml(receivedMessage.ToString());
                monitoringMessageJson = Encoding.UTF8.GetString( Convert.FromBase64String(document.InnerText));
            }

            var sanitisedMessageJson = monitoringMessageJson
                .Trim(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())
                    .ToCharArray());

            return sanitisedMessageJson;

            //byte[] messageBody;
            //if (transportEncoding.Equals("wcf/byte-array", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new InvalidOperationException("Legacy Xml messages not supported for ")
                 
            //    //var doc = receivedMessage.GetBody<XmlElement>();
            //    //messageBody = Convert.FromBase64String(doc.InnerText);
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