using System;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.Infrastructure.Messaging
{
    public static class LogicalMessageExtensions
    {
        //TODO: need to duplicate both methods for now until NSB has base class/interface for Incoming/Outgoing Logical Messages.  
        public static Guid GetMessageId(this IIncomingLogicalMessageContext context)
        {
            return (context.Message.Instance as IEvent)?.EventId ??
                   (context.Message.Instance as IPaymentsCommand)?.CommandId ?? Guid.Empty;
        }

        public static string GetMessageName(this IIncomingLogicalMessageContext context)
        {
            return context.Message.Instance.GetType().Name;
        }

        public static Guid GetMessageId(this IOutgoingLogicalMessageContext context)
        {
            return (context.Message.Instance as IEvent)?.EventId ??
                   (context.Message.Instance as IPaymentsCommand)?.CommandId ?? Guid.Empty;
        }

        public static string GetMessageName(this IOutgoingLogicalMessageContext context)
        {
            return context.Message.Instance.GetType().Name;
        }
    }
}