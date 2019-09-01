using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging
{
    public class JobStatusIncomingMessageBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly IJobMessageClientFactory factory;

        public JobStatusIncomingMessageBehaviour(IJobMessageClientFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var generatedMessages = new List<GeneratedMessage>();
            var paymentMessage = context.Message.Instance as IJobMessage;
            if (paymentMessage != null)
            {
                context.Extensions.Set(JobStatusBehaviourConstants.GeneratedMessagesKey, generatedMessages);
            }

            await next().ConfigureAwait(false);

            if (paymentMessage == null)
                return;
            var jobStatusClient = factory.Create();
            if (paymentMessage is ILeafLevelMessage)
            {
                await jobStatusClient.ProcessedCompletedJobMessage(paymentMessage.JobId, context.GetMessageId(), paymentMessage.GetType().Name, !generatedMessages.Any()).ConfigureAwait(false);
                return;
            }
            if (generatedMessages.Any())
                await jobStatusClient.RecordStartedProcessingJobMessages(paymentMessage.JobId,  generatedMessages)
                    .ConfigureAwait(false);
        }
    }
}