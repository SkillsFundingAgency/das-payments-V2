using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class ExceptionHandlingBehavior : Behavior<ITransportReceiveContext>
    {
        private readonly IPaymentLogger logger;

        public ExceptionHandlingBehavior(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
        {
            try
            {
                await next().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var messageType = "No enclosed messages";
                if (context.Message.Headers.ContainsKey("NServiceBus.EnclosedMessageTypes"))
                {
                    messageType = context.Message.Headers["NServiceBus.EnclosedMessageTypes"];
                }

                var sendingEndpoint = "No sending endpoint";
                if (context.Message.Headers.ContainsKey("NServiceBus.OriginatingEndpoint"))
                {
                    sendingEndpoint = context.Message.Headers["NServiceBus.OriginatingEndpoint"];
                }

                logger.LogError($"Message handling failed. Error: {ex.Message}\n\n" +
                                $"Message type: {messageType}\n\n" +
                                $"Sending Endpoint: {sendingEndpoint}\n\n", ex);
                throw;
            }
        }
    }
}