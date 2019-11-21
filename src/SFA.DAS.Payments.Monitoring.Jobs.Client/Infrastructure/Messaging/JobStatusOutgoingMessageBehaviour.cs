using NServiceBus.Pipeline;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging
{
    public class JobStatusOutgoingMessageBehaviour : Behavior<IOutgoingLogicalMessageContext>
    {
        public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            await next().ConfigureAwait(false);

            if (context.Message.Instance is IMonitoredMessage &&
                context.Extensions.TryGet(JobStatusBehaviourConstants.GeneratedMessagesKey, out List<GeneratedMessage> generatedMessages))
            {
                generatedMessages.Add(new GeneratedMessage
                {
                    StartTime = DateTimeOffset.UtcNow,
                    MessageId = context.GetMessageId(),
                    MessageName = context.GetMessageName()
                });
            }
        }
    }
}