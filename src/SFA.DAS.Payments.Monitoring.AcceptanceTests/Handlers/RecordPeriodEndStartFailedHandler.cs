using System.Collections.Concurrent;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.Monitoring.AcceptanceTests.Handlers
{
    public class RecordPeriodEndStartFailedHandler : IHandleMessages<RecordPeriodEndRunStartJobFailed>
    {


        public static ConcurrentBag<RecordPeriodEndRunStartJobFailed> ReceivedEvents { get; } = new ConcurrentBag<RecordPeriodEndRunStartJobFailed>();

        public Task Handle(RecordPeriodEndRunStartJobFailed message, IMessageHandlerContext context)
        {

            //This should be using a custom event rather than command. Need to create the event objects
            // and also check where these will be handled


            //ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}