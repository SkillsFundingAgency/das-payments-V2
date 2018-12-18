using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Client.Infrastructure.Messaging
{
    public class JobStatusIncomingMessageBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly IProviderEarningsJobStatusClientFactory factory;

        public JobStatusIncomingMessageBehaviour(IProviderEarningsJobStatusClientFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var eventIds = new List<(DateTimeOffset startTime, Guid id)>();
            var paymentMessage = context.Message.Instance as IPaymentsMessage;
            if (paymentMessage != null)
            {
                context.Extensions.Set("event_ids", eventIds);
            }

            await next().ConfigureAwait(false);

            if (paymentMessage == null)
                return;
            var jobStatusClient = factory.Create();
            await jobStatusClient.ProcessedJobMessage(paymentMessage.JobId,
                (paymentMessage as IPaymentsEvent)?.EventId ??
                (paymentMessage as PaymentsCommand)?.CommandId ?? Guid.Empty,
                eventIds).ConfigureAwait(false);
        }
    }
}