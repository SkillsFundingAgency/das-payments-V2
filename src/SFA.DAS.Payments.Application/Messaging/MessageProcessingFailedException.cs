using System;
using System.Runtime.Serialization;
using NServiceBus.Transport;

namespace SFA.DAS.Payments.Application.Messaging
{
    [Serializable]
    internal class MessageProcessingFailedException : Exception
    {
        private readonly string message;
        private readonly IncomingMessage incomingMessage;
        private readonly Exception innerException;


        public MessageProcessingFailedException(string message, IncomingMessage incomingMessage, Exception innerException) : base(message, innerException)
        {
            this.message = message;
            this.incomingMessage = incomingMessage;
            this.innerException = innerException;
        }

        protected MessageProcessingFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}