using NServiceBus.Pipeline;
using System;
using System.Runtime.Serialization;

namespace SFA.DAS.Payments.Application.Messaging
{
    [Serializable]
    internal class MessageProcessingFailedException : Exception
    {
        private readonly string message;
        private readonly LogicalMessage incomingMessage;
        private readonly Exception innerException;


        public MessageProcessingFailedException(string message, LogicalMessage incomingMessage, Exception innerException) : base(message, innerException)
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