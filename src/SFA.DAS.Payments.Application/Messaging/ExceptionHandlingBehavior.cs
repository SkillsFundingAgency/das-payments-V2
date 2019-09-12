using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;

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
                if (!(context.Message.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out var messageType) 
                    && TypeString.TryParseTypeName(messageType, out messageType)))
                {
                    messageType = "No enclosed messages";
                }

                if (!context.Message.Headers.TryGetValue("NServiceBus.OriginatingEndpoint", out var sendingEndpoint))
                    sendingEndpoint = "No sending endpoint";

                throw new MessageProcessingFailedException($"Couldn't process message `{messageType}` from `{sendingEndpoint}`.", context.Message, ex);
            }
        }
    }

}